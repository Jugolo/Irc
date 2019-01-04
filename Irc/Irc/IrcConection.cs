using Irc.Irc.IS;
using Irc.Script;
using Irc.Script.Types;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Irc.Irc
{
    public class IrcConection
    {
        public delegate void IRCEventListner(IrcConection connection, IrcMessage message);

        private TcpClient client;
        private StreamWriter writer;
        private StreamReader reader;
        private IRCEventListner events;
        public Dictionary<string, EcmaHeadObject> scriptEvenets = new Dictionary<string, EcmaHeadObject>();
        public Dictionary<string, EcmaHeadObject> scriptAction = new Dictionary<string, EcmaHeadObject>();
        private string[] Channels { get; set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public IrcScript script { get; private set; }
        private string nick;
        private string identify;
        private Form1 Main;
        private Thread thread;
        private Dictionary<string, WhoIsData> whois = new Dictionary<string, WhoIsData>();
        
        public ConnectionStatus Status { get; set; }

        public IrcConection(Form1 main, string identify, string host, int port, string nick, string[] channels)
        {
            this.Host = host;
            this.Port = port;
            this.Main = main;
            this.nick = nick;
            this.identify = identify;
            this.Channels = channels;
            this.Status = ConnectionStatus.Paused;
        }

        public void Connect() {
            thread = new Thread(DoConnect);
            thread.Start();
        }

        private void DoConnect() { 
            if (this.Status == ConnectionStatus.Startet)
                return;

            int i = 0;
            for (; i <= 10; i++)
            {
                try
                {
                    this.client = new TcpClient(this.Host, this.Port);
                    this.writer = new StreamWriter(this.client.GetStream());
                    this.reader = new StreamReader(this.client.GetStream());
                    break;
                }catch(IOException e)
                {
                    this.Main.channel1.Write(this.identify, "*", "", '\x003'.ToString()+"4,0Could not connect: "+e.Message);
                }catch(SocketException e)
                {
                    this.Main.channel1.Write(this.identify, "*", "", '\x003'.ToString()+"4,0Could not connect: " + e.Message);
                }
                Thread.Sleep(5000);
            }
            if(i == 10)
            {
                this.Main.channel1.Write(this.identify, "*", "", '\x003'.ToString()+"4,0Could not connect to the server.");
                return;
            }
            this.Main.channel1.Write(this.identify, "*", "", '\x003'.ToString()+"3,0Is now connected");
            this.Status = ConnectionStatus.Startet;
            this.script = new IrcScript(this, this.Main);
            this.Open();
        }

        public string GetNick()
        {
            return this.nick;
        }

        public void SetNick(string nick)
        {
            this.nick = nick;
        }

        public string GetIdentify()
        {
            return this.identify;
        }

        private void Open()
        {

            //Send the init message (Nick and User)
            this.SendLine("USER " + nick + " 0 * :" + nick);
            this.SendLine("NICK " + nick);
            this.Flush();
            this.Start();
        }

        public void Pause()
        {
            this.Status = ConnectionStatus.Paused;
            this.reader.Dispose();
        }

        private void Start() { 

            string line;
            try
            {
                while (this.Status == ConnectionStatus.Startet && (line = this.ReadLine()) != null)
                {
                    if (line.IndexOf("PING :") == 0)
                    {
                        this.SendLine("PONG" + line.Substring(4));
                        this.Flush();
                        continue;
                    }
                   
                    this.HandleMessage(IrcMessage.Parse(line));
                }
            }catch(IOException)
            {

            }
        }

        public string ReadLine()
        {
            if(this.reader != null)
                return this.reader.ReadLine();
            return null;
        }

        public void SendLine(string line)
        {
            if(this.writer != null)
                this.writer.Write(line+"\n");
        }

        public void Flush()
        {
            if(this.writer != null && this.writer.BaseStream != null)
                this.writer.Flush();
        }

        public void Close()
        {
            if (this.Status != ConnectionStatus.Startet)
                return;

            this.SendLine("QUIT :");
            this.Flush();
            if(this.writer != null)
            {
                this.writer.Dispose();
                this.writer = null;
            }
            if (this.reader != null)
            {
                this.reader.Dispose();
                this.reader = null;
            }
            this.client.Close();
        }

        public void SetEventListener(IRCEventListner evenets)
        {
            this.events = evenets;
        }

        private void HandleMessage(IrcMessage message)
        {
            //if message is null we do not handle the request
            if (message == null)
                return;

            //let us se if this is a type as int
            double d;
            if (double.TryParse(message.Type, out d))
            {
                switch (message.Type)
                {
                    case "001":
                        foreach(string channel in this.Channels)
                        {
                            this.SendLine("JOIN " + channel);
                        }
                        this.Flush();
                        break;
                    case "301":
                        nick = message.ParamsMidle.Split(' ')[1];
                        if (this.whois.ContainsKey(nick))
                        {
                            whois[nick].Away = true;
                            whois[nick].AwayMessage = message.ParamsTrailing;
                        }
                        break;
                    case "311":
                        string[] wn = message.ParamsMidle.Split(' ');
                        WhoIsData wd = new WhoIsData();
                        wd.Nick = wn[1];
                        if (this.whois.ContainsKey(wn[1]))
                            this.whois[wn[1]] = wd;
                        else
                            this.whois.Add(wn[1], wd);
                        break;
                    case "317":
                        nick = message.ParamsMidle.Split(' ')[1];
                        if (this.whois.ContainsKey(nick))
                        {

                        }
                        break;
                    case "318":
                        nick = message.ParamsMidle.Split(' ')[1];
                        if (this.whois.ContainsKey(nick))
                        {
                            if (this.scriptAction.ContainsKey("server.whois"))
                            {
                                this.DoAction("server.whois", new ScriptWhoIsData(this.script.State, this.whois[nick]));
                            }
                        }
                        break;
                    case "319":
                        nick = message.ParamsMidle.Split(' ')[1];
                        if (this.whois.ContainsKey(nick))
                        {
                            this.whois[nick].Channels = message.ParamsTrailing.Split(' ');
                        }
                        break;
                    case "332":
                        this.Main.SetTopic(this.identify, message.ParamsMidle.Substring(message.ParamsMidle.LastIndexOf(' ') + 1), message.ParamsTrailing);
                        break;
                    case "353":
                        string[] users = message.ParamsTrailing.Split(' ');
                        for(int i = 0; i < users.Length; i++)
                        {
                            char prefix = users[i].ToCharArray()[0];
                            UserInfo info = new UserInfo();
                            if(prefix == '+' || prefix == '@')
                            {
                                info.Nick = users[i].Substring(1);
                                info.Op = prefix == '@';
                                info.Voice = prefix == '+';
                            }
                            else
                            {
                                info.Nick = users[i];
                            }
                            this.Main.AppendUser(this.identify, message.ParamsMidle.Substring(message.ParamsMidle.LastIndexOf(' ') + 1), info);

                            if (this.scriptEvenets.ContainsKey("353"))
                            {
                                this.DoPlugin("353", new ScriptUserInfo(info));
                            }
                        }
                        break;
                }

                if (this.events != null)
                    this.events(this, message);
            }
            else
            {
                this.events?.Invoke(this, message);

                if (this.scriptEvenets.ContainsKey(message.Type))
                {
                    this.DoPlugin(message.Type, new IrcScriptMessage(message));
                }
            }
        }

        private void DoPlugin(string type, EcmaHeadObject message)
        {
            if (scriptEvenets[type] is ICallable)
            {
                (scriptEvenets[type] as ICallable).Call(null, new EcmaValue[]
                    {
                    EcmaValue.Object(message)
                    });
            }
        }

        public void DoAction(string type, EcmaHeadObject data)
        {
            if (scriptAction[type] is ICallable)
            {
                (scriptAction[type] as ICallable).Call(null, new EcmaValue[]{
                    EcmaValue.Object(data)
                });
            }
        }
    }
}

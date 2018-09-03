using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using torrent.Script;
using torrent.Script.Values;

namespace Irc.Irc
{
    public class IrcConection
    {
        public delegate void IRCEventListner(IrcConection connection, IrcMessage message);

        private TcpClient client;
        private StreamWriter writer;
        private StreamReader reader;
        private IRCEventListner events;
        public Dictionary<string, FunctionInstance> scriptEvenets = new Dictionary<string, FunctionInstance>();
        public Dictionary<string, FunctionInstance> scriptAction = new Dictionary<string, FunctionInstance>();
        private string[] Channels { get; set; }
        private IrcScript script;
        private string nick;
        private string[] channels;
        private string identify;
        private Form1 Main;
        
        public ConnectionStatus Status { get; set; }

        public IrcConection(Form1 main, string identify, string host, int port, string nick, string[] channels)
        {
            this.Main = main;
            this.Status = ConnectionStatus.Startet;
            this.nick = nick;
            this.channels = channels;
            this.identify = identify;
            this.client = new TcpClient(host, port);
            this.writer = new StreamWriter(this.client.GetStream());
            this.reader = new StreamReader(this.client.GetStream());
            this.Channels = channels;
            this.script = new IrcScript(this, main);

            Thread thread = new Thread(Open);
            thread.Start();
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

        public void Start() { 

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
                    this.DoPlugin(message);
                }
            }
        }

        private void DoPlugin(IrcMessage message)
        {
            this.scriptEvenets[message.Type].Call(new Value[] { new IrcScriptMessage(message) });
        }
    }
}

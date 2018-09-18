using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Irc.Database;
using Irc.Forms;
using Irc.Irc;
using torrent.Script.Values;

namespace Irc
{
    public partial class Form1 : Form
    {
        delegate void AN(string identify, string channel);
        delegate void UT(string topic);
        private Servers Server { get; set; }
        private List<IrcConection> connections = new List<IrcConection>();
        private Dictionary<string, TreeNode> Nodes = new Dictionary<string, TreeNode>();
        private Dictionary<string, List<IrcChannel>> channel = new Dictionary<string, List<IrcChannel>>();

        public Form1()
        {
            this.Server = new Servers();
            InitializeComponent();
            this.channel1.SetSendMessageEvent(GetSendMessage);
        }

        public void SetTopic(string identify, string channel, string topic)
        {
            this.channel1.SetTopic(identify, channel, topic);
            this.SetTitle();
        }

        public void SetTitle()
        {
            string topic = "Not connected";

            if(this.channel1.SelectedIdentify() != null)
            {
                topic = "[" + this.channel1.SelectedIdentify() + "]";
                if(this.channel1.SelectedChannel() != null)
                {
                    if (this.channel1.SelectedChannel() != "*")
                    {
                        topic += " " + this.channel1.SelectedChannel();
                        string t = this.channel1.GetCurrentTopic();
                        if(t != null)
                        {
                            topic += ": " + t;
                        }
                    }
                    else
                    {
                        topic += " Main server";
                    }
                }
                else
                {
                    topic += " No joined channels";
                }
            }
            this.UpdateTopic(topic);
        }

        public void AppendUser(string identify, string channel, UserInfo info)
        {
            this.channel1.AppendUser(identify, channel, info);
        }

        public void SaveServer(string name, string host, int port, string nick, string[] channels)
        {
            this.Server.Save(name, host, port, nick, channels);
            this.OpenConnection(name, host, port, nick, channels);
        }

        public bool CloseConnection(string identify)
        {
            IrcConection connection = this.GetConnection(identify);
            if(connection == null)
            {
                return false;
            }

            connection.Close();
            this.Server.DeleteConnection(identify);
            this.Nodes[identify].Remove();
            this.channel1.CloseServer(identify);
            if(this.channel1.SelectedIdentify() == identify)
            {
                string ns = this.channel1.GetTopServer();
                if(ns == null)
                {
                    this.OpenNewServerDialog(); 
                    return true;
                }
                this.channel1.Select(ns, "*");
            }
            return true;
        }

        public bool CloseChannel(string identify, string name)
        {
            if(!this.channel1.CloseChannel(identify, name))
            {
                return false;
            }

            if (name.IndexOf("#") == 0)
            {
                IrcConection connection = this.GetConnection(identify);
                connection.SendLine("PART " + name);
                connection.Flush();
            }

            foreach(TreeNode n in this.Nodes[identify].Nodes)
            {
                if(n.Text == name)
                {
                    n.Remove();
                    break;
                }
            }

            this.Server.DeleteChannel(identify, name);

            if(this.channel1.SelectedIdentify() == identify && this.channel1.SelectedChannel() == name)
            {
                this.channel1.Select(this.channel1.SelectedIdentify(), this.channel1.GetTopChannel(identify));
            }
            return true;
        }

        public IrcConection GetConnection(string name)
        {
            for(int i = 0; i < this.connections.Count; i++)
            {
                if(this.connections[i].GetIdentify() == name)
                {
                    return this.connections[i];
                }
            }
            return null;
        }

        public void GetSendMessage(string identify, string channel, string message)
        {
            IrcConection connection = GetConnection(identify);
            if (connection == null)
                return;

            if(message.TrimStart().IndexOf("/") == 0)
            {
                int space = message.IndexOf(' ');
                string command;
                if (space == -1)
                    command = message.Substring(1).Trim();
                else
                    command = message.Substring(1, Math.Max(0, space - 1)).ToLower().Trim();
                string[] param = space == -1 ? new string[0] : message.Substring(space + 1).Split(' ');
                switch (command)
                {
                    case "join":
                        foreach (string c in param)
                        {
                            if (c.IndexOf("#") == 0)
                                connection.SendLine("JOIN " + c);
                        }
                        connection.Flush();
                        break;
                    case "leave":
                        if(channel == "*")
                        {
                            this.CloseConnection(identify);
                        }else if(channel.IndexOf("#") == 0)
                        {
                            this.CloseChannel(identify, channel);
                        }
                        break;
                    case "nick":
                        if (param.Length == 0)
                            return;
                        connection.SendLine("NICK " + param[0]);
                        connection.Flush();
                        break;
                    default:
                        command = command.ToUpper();
                        StringBuilder p = new StringBuilder();
                        for(int i = 0; i < param.Length; i++)
                        {
                            p.Append(" "+param[i]);
                        }
                        connection.SendLine(command + p.ToString());
                        connection.Flush();
                        break;
                }
            }
            else
            {
                if (channel != "*")
                {
                    connection.SendLine("PRIVMSG " + channel + " :" + message);
                    connection.Flush();
                    this.channel1.ShowLine(identify, channel, connection.GetNick(), message);
                }
                else
                {
                    this.channel1.ShowLine(identify, "*", "", IrcUntil.ColoeredText(4, 0, "You can not send message in server windo"));
                }
            }
        }

        public void MarkRead(string identify, string channel)
        {
            if (this.Nodes.ContainsKey(identify))
            {
                foreach(TreeNode node in this.Nodes[identify].Nodes)
                {
                    if(channel == node.Text)
                    {
                        node.ForeColor = Color.Black;
                    }
                }
            }
        }

        public void MarkUnread(string identify, string channel)
        {
            if (this.Nodes.ContainsKey(identify))
            {
                foreach(TreeNode n in this.Nodes[identify].Nodes)
                {
                    if(n.Text == channel)
                    {
                        n.ForeColor = Color.Red;
                        return;
                    }
                }
            }
        }

        private void OnEvents(IrcConection connection, IrcMessage message)
        {
            this.channel1.ShowLine(connection.GetIdentify(), "*", "", message.Raw);
            string channel;
            switch (message.Type)
            {
                case "JOIN":
                    //Let us se if this is us there join a channel or it is a user
                    if(message.Nick == connection.GetNick())
                    {
                        channel = message.ParamsMidle;
                        this.AppendChannel(connection.GetIdentify(), channel);
                        this.Server.SaveChannel(connection.GetIdentify(), channel);
                    }
                    else if(message.ParamsMidle.IndexOf("#") == 0)
                    {
                        this.channel1.ShowLine(connection.GetIdentify(), message.ParamsMidle, "", IrcUntil.ColoeredText(9, 0, message.Nick + " came into this room"));
                        UserInfo info = new UserInfo();
                        info.Nick = message.Nick;
                        this.channel1.AppendUser(connection.GetIdentify(), message.ParamsMidle, info);
                    }
                    break;
                case "PART":
                    channel = message.ParamsMidle.Substring(1);
                    this.channel1.ShowLine(connection.GetIdentify(), channel, "", IrcUntil.ColoeredText(4, 0, message.Nick+" leaved this channel"));
                    this.channel1.RemoveUser(connection.GetIdentify(), message.ParamsMidle.Substring(1), message.Nick);
                    break;
                case "PRIVMSG":
                    if(message.ParamsMidle == connection.GetNick())
                    {
                        //this is a private message from user (channel start width #) so wee controle we have the channel. 
                        if (!this.ChannelExists(connection.GetIdentify(), message.Nick))
                        {
                            this.AppendChannel(connection.GetIdentify(), message.Nick);
                            this.channel1.Select(connection.GetIdentify(), message.Nick);
                        }
                    }
                    if(message.ParamsTrailing.IndexOf('\x001'.ToString()) == 0)
                    {
                        if(message.ParamsTrailing.IndexOf('\x001'.ToString()+"ACTION") == 0)
                        {
                            this.channel1.ShowLine(connection.GetIdentify(), message.ParamsTrailing == connection.GetNick() ? message.Nick : message.ParamsMidle, null, IrcUntil.ColoeredText(6, 0, "* " + message.Nick + " " + message.ParamsTrailing.Substring(8)));
                            return;
                        }
                    }
                    this.channel1.ShowLine(connection.GetIdentify(), message.ParamsMidle == connection.GetNick() ? message.Nick : message.ParamsMidle, message.Nick, message.ParamsTrailing);
                    break;
                case "NICK":
                    string n = message.ParamsTrailing;
                    if(n == connection.GetNick())
                    {
                        //this is my there will or foreced to chage nick name. 
                        connection.SetNick(n);
                        this.Server.UpdateNick(connection.GetIdentify(), n);
                    }
                    this.channel1.UdateNick(connection.GetIdentify(), message.Nick, n);
                    break;
            }
        }

        private void OpenConnection(string identify, string host, int port, string nick, string[] channels)
        {
            IrcConection connection = new IrcConection(this, identify, host, port, nick, channels);
            connection.SetEventListener(OnEvents);
            this.treeView1.BeginUpdate();
            TreeNode node = this.treeView1.Nodes.Add(identify);
            this.Nodes.Add(identify, node);
            this.treeView1.EndUpdate();
            this.treeView1.SelectedNode = node;
            this.connections.Add(connection);
            this.channel1.Select(identify, "*");
            this.channel1.ShowLine(identify, "*", "", "Open connection");
            connection.Connect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Form1_Resize(this, new EventArgs());
            int count = Server.GetServerCount();
            if(count == 0)
            {
                OpenNewServerDialog();
            }
            else
            {
                Server.ForeachServers(OpenConnection);
            }
        }

        private void OpenNewServerDialog()
        {
            NewServer server = new NewServer(this);
            server.Show(this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (IrcConection client in this.connections)
                client.Close();
        }

        private bool ChannelExists(string identify, string channel)
        {
            return this.channel1.ChannelExists(identify, channel);
        }

        public void AppendChannel(string identify, string channel)
        {
            if (!this.channel.ContainsKey(identify))
                this.channel.Add(identify, new List<IrcChannel>());

            this.channel[identify].Add(new IrcChannel(this, channel, this.GetConnection(identify)));

            this.channel1.Select(identify, channel);
            if(channel.IndexOf("#") == 0)
                this.channel1.ShowLine(identify, channel, "", "You joined the channel");

            this.treeView1.BeginInvoke(new AN(AppendChannelNode), new object[]
            {
                identify,
                channel
            });

            IrcConection connection = this.GetConnection(identify);
            if (connection.scriptAction.ContainsKey("channel.open"))
            {
                connection.scriptAction["channel.open"].Call(new Value[]
                {
                    new StringValue(channel)
                });
            }
        }

        private void AppendChannelNode(string identify, string channel)
        {
            TreeNode node = this.Nodes[identify].Nodes.Add(channel);
            this.treeView1.SelectedNode = node;
            this.Nodes[identify].ExpandAll();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.channel1.Width = this.Width - this.treeView1.Width;
            this.channel1.Height = this.ClientRectangle.Height;
        }

        private void treeViewSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)
                this.channel1.Select(e.Node.Parent.Text, e.Node.Text);
            else
                this.channel1.Select(e.Node.Text, "*");
        }

        private void connectToNewServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenNewServerDialog();
        }

        private void UpdateTopic(string topic)
        {
            this.BeginInvoke(new UT(UpdateTopicNow), new object[] {
                topic
            });
        }

        private void UpdateTopicNow(string topic)
        {
            this.Text = topic;
        }
    }
}

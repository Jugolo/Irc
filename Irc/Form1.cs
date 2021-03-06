﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Irc.Forms;
using Irc.Irc;
using Irc.Script;
using Irc.Script.Types;
using Irc.Script.Types.Function;

namespace Irc
{
    public partial class Form1 : Form
    {
        private delegate void NodeUpdater(string server, string channel);
        private delegate void UpdateTopic();

        private List<IrcConection> connections = new List<IrcConection>();
        private Dictionary<string, TreeNode> Nodes = new Dictionary<string, TreeNode>();
        private EcmaScript Client = new EcmaScript();

        private EcmaHeadObject ScriptClient = new EcmaHeadObject();

        public string GetIdentify
        {
            get  { return this.channel1.SelectedServer; }
        }

        public Form1()
        {
            this.Client.BuildStandartLibary();
            new DefaultScript(this.Client);

            ScriptClient.Put("showServerDialog", EcmaValue.Object(new NativeFunctionInstance(1, this.Client.State, OpenNewServerDialog)));
            ScriptClient.Put("openConnection", EcmaValue.Object(new NativeFunctionInstance(5, this.Client.State, OpenConnection)));
            this.Client.CreateVariable("Client", EcmaValue.Object(ScriptClient));

            using (TextReader script = File.OpenText("Script/Client.js")) {
                this.Client.RunCode(script);
            }
            InitializeComponent();
        }

        public void SetTopic(string identify, string channel, string topic)
        {
            this.channel1.SetTopic(identify, channel, topic);
            this.SetTitle();
        }

        public void SetTitle()
        {
            string topic = "Not connected";

            if(this.channel1.SelectedServer != null)
            {
                topic = "[" + this.channel1.SelectedServer + "]";
                if(this.channel1.SelectedChannel != null)
                {
                    if (this.channel1.SelectedChannel != "*")
                    {
                        topic += " " + this.channel1.SelectedChannel;
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

            this.BeginInvoke(new UpdateTopic(() => {
                Text = topic;
            }));
        }

        public void AppendUser(string identify, string channel, UserInfo info)
        {
            this.channel1.AppendUser(identify, channel, info);
        }

        public void SaveServer(string name, string host, int port, string nick, string[] channels)
        {
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
            this.Nodes[identify].Remove();
            this.channel1.RemoveServer(identify);
            if(this.channel1.SelectedServer == identify)
            {
                string ns = this.channel1.GetTopServer();
                if(ns == null)
                {
                    //this.OpenNewServerDialog(); 
                    return true;
                }
                this.channel1.Select(ns, "*");
            }
            return true;
        }

        public bool CloseChannel(string identify, string name)
        {
            if(!this.channel1.RemoveChannel(identify, name))
            {
                return false;
            }

            if (name.IndexOf("#") == 0)
            {
                IrcConection connection = this.GetConnection(identify);
                connection.SendLine("PART " + name);
                connection.Flush();

                if (this.ScriptClient.HasProperty("onSelfPart"))
                {
                    EcmaHeadObject callable = this.ScriptClient.Get("onSelfPart").ToObject(this.Client.State);
                    if(callable is ICallable)
                    {
                        (callable as ICallable).Call(this.ScriptClient, new EcmaValue[]
                        {
                            EcmaValue.String(identify),
                            EcmaValue.String(name)
                        });
                    }
                }
            }

            foreach(TreeNode n in this.Nodes[identify].Nodes)
            {
                if(n.Text == name)
                {
                    n.Remove();
                    break;
                }
            }
            

            if(this.channel1.SelectedServer == identify && this.channel1.SelectedChannel == name)
            {
                this.channel1.Select(this.channel1.SelectedServer, this.channel1.GetTopChannel(identify));
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

        public void GetSendMessage(string message)
        {
            IrcConection connection = GetConnection(this.channel1.SelectedServer);
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
                    case "me":
                        connection.SendLine("PRIVMSG " + this.channel1.SelectedChannel + " :\x0001ACTION " + message.Substring(4)+ "\x0001");
                        connection.Flush();
                        this.channel1.Write(
                            this.channel1.SelectedServer,
                            this.channel1.SelectedChannel,
                            "",
                            IrcUntil.ColoeredText(6, 0, "* " + this.GetConnection(this.channel1.SelectedServer).GetNick() + " " + message.Substring(4))
                            );
                        break;
                    case "join":
                        foreach (string c in param)
                        {
                            if (c.IndexOf("#") == 0)
                                connection.SendLine("JOIN " + c);
                        }
                        connection.Flush();
                        break;
                    case "leave":
                        if(this.channel1.SelectedChannel == "*")
                        {
                            this.CloseConnection(this.channel1.SelectedServer);
                        }else if(this.channel1.SelectedChannel.IndexOf("#") == 0)
                        {
                            this.CloseChannel(this.channel1.SelectedServer, this.channel1.SelectedChannel);
                        }
                        break;
                    case "nick":
                        if (param.Length == 0)
                            return;
                        connection.SendLine("NICK " + param[0]);
                        connection.Flush();
                        break;
                    case "query":
                        if(param.Length > 0)
                        {
                            if(param[0][0] == '#')
                            {
                                this.channel1.Write(IrcUntil.ColoeredText(IrcColorPlate.Red, IrcColorPlate.White, "You an not query a channel. Use /join"));
                            }
                            else
                            {
                                if(param.Length == 1)
                                {
                                    this.AppendChannel(this.channel1.SelectedServer, param[0]);
                                }
                                else
                                {
                                    StringBuilder query = new StringBuilder();
                                    query.Append(param[1]);
                                    for(int i = 2; i < param.Length; i++)
                                    {
                                        query.Append(" " + param[i]);
                                    }
                                    this.AppendChannel(this.channel1.SelectedServer, param[0]);
                                    this.GetSendMessage(query.ToString());
                                }
                            }
                        }
                        else
                        {
                            this.channel1.Write(IrcUntil.ColoeredText(IrcColorPlate.Red, IrcColorPlate.White, "You need to write to who"));
                        }
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
                if (this.channel1.SelectedChannel != "*")
                {
                    connection.SendLine("PRIVMSG " + this.channel1.SelectedChannel + " :" + message);
                    connection.Flush();
                    this.channel1.Write(this.channel1.SelectedServer, this.channel1.SelectedChannel, connection.GetNick(), message);
                }
                else
                {
                    this.channel1.Write(this.channel1.SelectedServer, "*", "", IrcUntil.ColoeredText(4, 0, "You can not send message in server windo"));
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
            this.channel1.Write(connection.GetIdentify(), "*", "", message.Raw);
            string channel;
            switch (message.Type)
            {
                case "JOIN":
                    //Let us se if this is us there join a channel or it is a user
                    if(message.Nick == connection.GetNick())
                    {
                        channel = message.ParamsMidle;
                        this.AppendChannel(connection.GetIdentify(), channel);
                    }
                    else if(message.ParamsMidle.IndexOf("#") == 0)
                    {
                        this.channel1.Write(connection.GetIdentify(), message.ParamsMidle, "", IrcUntil.ColoeredText(9, 0, message.Nick + " came into this room"));
                        UserInfo info = new UserInfo();
                        info.Nick = message.Nick;
                        this.channel1.AppendUser(connection.GetIdentify(), message.ParamsMidle, info);
                    }
                    break;
                case "PART":
                    if (message.ParamsMidle == "")
                        return;
                    channel = message.ParamsMidle.Substring(1);
                    this.channel1.Write(connection.GetIdentify(), channel, "", IrcUntil.ColoeredText(4, 0, message.Nick+" leaved this channel"));
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
                            this.channel1.Write(connection.GetIdentify(), message.ParamsTrailing == connection.GetNick() ? message.Nick : message.ParamsMidle, null, IrcUntil.ColoeredText(6, 0, "* " + message.Nick + " " + message.ParamsTrailing.Substring(8)));
                            return;
                        }
                    }
                    this.channel1.Write(connection.GetIdentify(), message.ParamsMidle == connection.GetNick() ? message.Nick : message.ParamsMidle, message.Nick, message.ParamsTrailing);
                    break;
                case "NICK":
                    string n = message.ParamsTrailing;
                    if(n == connection.GetNick())
                    {
                        //this is my there will or foreced to chage nick name. 
                        connection.SetNick(n);
                    }
                    this.channel1.UpdateNick(connection.GetIdentify(), message.Nick, n);
                    break;
            }
        }

        private EcmaValue OpenConnection(EcmaHeadObject self, EcmaValue[] arg)
        {
            EcmaState state = this.Client.State;

            OpenConnection(
                arg[0].ToString(state),
                arg[1].ToString(state),
                arg[2].ToInt32(state),
                arg[3].ToString(state),
                EcmaUntil.ToStringArray(state, arg[4].ToObject(state))
                );
            return EcmaValue.Undefined();
        }

        private void OpenConnection(string identify, string host, int port, string nick, string[] channels)
        {
            IrcConection connection = new IrcConection(this, identify, host, port, nick, channels);
            connection.SetEventListener(OnEvents);
            this.channel1.CreateServer(identify, nick);
            this.treeView1.BeginUpdate();
            TreeNode node = this.treeView1.Nodes.Add(identify);
            this.Nodes.Add(identify, node);
            this.treeView1.EndUpdate();
            this.treeView1.SelectedNode = node;
            this.connections.Add(connection);
            connection.Connect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Form1_Resize(this, new EventArgs());
            EcmaHeadObject Main = this.Client.GetVariabel("main").ToObject(this.Client.State);
            (Main as ICallable).Call(Main, new EcmaValue[0]);
        }

        private EcmaValue OpenNewServerDialog(EcmaHeadObject self, EcmaValue[] args)
        {
            NewServer server = new NewServer(args[0].ToObject(this.Client.State), this.Client.State);
            server.Show(this);
            return EcmaValue.Undefined();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (IrcConection client in this.connections)
                client.Close();
        }

        private bool ChannelExists(string identify, string channel)
        {
            return this.channel1.IsChannelExists(identify, channel);
        }

        public void AppendChannel(string identify, string channel)
        {
            this.channel1.CreateChannel(identify, channel);

            this.AppendNodeChannel(identify, channel);

            if (channel.IndexOf("#") == 0)
            {
                this.channel1.Write(identify, channel, "", "You joined the channel");
            }

            if (this.ScriptClient.HasProperty("onOwnJoin"))
            {
                EcmaHeadObject callable = this.ScriptClient.Get("onOwnJoin").ToObject(Client.State);
                if(callable is ICallable)
                {
                    (callable as ICallable).Call(this.ScriptClient, new EcmaValue[]
                    {
                        EcmaValue.String(identify),
                        EcmaValue.String(channel)
                    });
                }
            }

            
            IrcConection connection = this.GetConnection(identify);
            if (connection.scriptAction.ContainsKey("channel.open"))
            {
                connection.DoAction("channel.open", EcmaValue.String(channel).ToObject(connection.script.State));
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
            if (this.ScriptClient.HasProperty("onServerRequest"))
            {
                EcmaValue value = this.ScriptClient.Get("onServerRequest");
                if (value.IsObject())
                {
                    EcmaHeadObject obj = value.ToObject(this.Client.State);
                    if(obj is ICallable)
                    {
                        (obj as ICallable).Call(obj, new EcmaValue[0]);
                        return;
                    }
                }
            }
        }

        private void UpdateTopicNow(string topic)
        {
            this.Text = topic;
        }

        private void AppendNodeChannel(string server, string channel)
        {
            this.treeView1.BeginInvoke(new NodeUpdater(AppendNode), new object[]
            {
                server,
                channel
            });
        }

        private void AppendNode(string server, string channel)
        {
            this.treeView1.BeginUpdate();
            this.Nodes[server].Nodes.Add(channel);
            this.treeView1.EndUpdate();
            this.Nodes[server].Expand();
        }
    }
}

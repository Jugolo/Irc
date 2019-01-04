using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irc.Irc;

namespace Irc.Forms
{
    public partial class Channel : UserControl
    {
        private Form1 main;
        private IrcServerInfo Info = new IrcServerInfo();
        

        public string SelectedServer { get; private set; }
        public string SelectedChannel { get; private set; }
        public string Nick { get { return this.Info.GetNick(this.SelectedServer); } }

        public Channel(Form1 main)
        {
            this.main = main;
            InitializeComponent(main);
        }

        public void CreateServer(string server, string nick)
        {
            this.Info.CreateServer(server, nick, this.message1, this.UserList);
            this.Select(server, "*");
            this.Write(server, "*", "", "Open connection");
        }

        public void SetTopic(string identify, string channel, string topic)
        {
            this.Info.SetTopic(identify, channel, topic);
            this.Write(identify, channel, "", topic);
        }

        public string GetCurrentTopic()
        {
            return this.Info.GetTopic(this.SelectedServer, this.SelectedChannel);
        }

        public void AppendUser(string server, string channel, UserInfo info)
        {
            this.Info.AppendUser(server, channel, info);
        }

        public void RemoveUser(string server, string channel, string nick)
        {
            this.Info.RemoveUser(server, channel, nick);
        }

        public void UpdateNick(string server, string nick, string newNick)
        {
            this.Info.UpdateNick(server, nick, newNick);
        }

        public void RemoveServer(string name)
        {
            this.Info.RemoveServer(name);
        }

        public string GetTopServer()
        {
            return this.Info.GetTopServer();
        }

        public bool RemoveChannel(string server, string channel)
        {
            return this.Info.RemoveChannel(server, channel);
        }

        public string GetTopChannel(string server)
        {
            return this.Info.GetTopChannel(server);
        }

        public void Select(string server, string channel)
        {
            if(this.SelectedServer != null && this.SelectedChannel != null)
                this.Info.UnSelect(this.SelectedServer, this.SelectedChannel);
            this.SelectedServer = server;
            this.SelectedChannel = channel;
            this.main.SetTitle();
            this.Info.Select(server, channel);
        }

        public void Write(string message)
        {
            this.Write(
                this.SelectedServer,
                this.SelectedChannel,
                this.Nick,
                message
                );
        }

        public void Write(string server, string channel, string nick, string message)
        {
            this.Info.Write(server, channel, nick, message);
        }

        public bool IsChannelExists(string server, string channel)
        {
            return this.Info.IsChannelExists(server, channel);
        }

        public void CreateChannel(string server, string channel)
        {
            this.Info.CreateChannel(server, channel);
            this.Select(server, channel);
        }
    }
}

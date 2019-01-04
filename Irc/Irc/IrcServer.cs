using Irc.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc
{
    class IrcServer
    {
        public string Identify { get; private set; }
        public string Nick { get; private set; }

        private List<IrcChannel> Channels = new List<IrcChannel>();
        private Message Message;
        private UserList userlist;

        private delegate void DummyDelegate();

        public IrcServer(string identify, string nick, Message message, UserList userlist)
        {
            this.Identify = identify;
            this.Message = message;
            this.userlist = userlist;
            this.Nick = nick;
            this.CreateChannel("*");
        }

        public void Select(string channel)
        {
            this.userlist.BeginInvoke(new DummyDelegate(() =>
            {
                if (channel.IndexOf("#") == 0)
                    this.userlist.Show();
                else
                    this.userlist.Hide();
            }));

            this.GetChannel(channel).Select();
        }

        public void UnSelect(string channel)
        {

            this.GetChannel(channel)?.UnSelect();
        }

        public bool RemoveChannel(string name)
        {
            for(int i = 0; i < Channels.Count; i++)
            {
                if(Channels[i].Name == name)
                {
                    Channels.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public string GetTopChannel()
        {
            if (Channels.Count == 0)
                return null;
            return Channels[0].Name;
        }

        public void Write(string channel, string nick, string message)
        {
            GetChannel(channel)?.Write(nick, message);
        }

        public void UpdateNick(string nick, string newNick)
        {
            if (this.Nick == nick)
                this.Nick = newNick;

            for(int i = 0; i < Channels.Count; i++)
            {
                Channels[i].UpdateNick(nick, newNick);
            }
        }

        public bool IsChannelExists(string channel)
        {
            return GetChannel(channel) != null;
        }

        public void CreateChannel(string channel)
        {
            this.Channels.Add(new IrcChannel(channel, this.Message, this.userlist));
        }

        public IrcChannel GetChannel(string name)
        {
            for(int i = 0; i < Channels.Count; i++)
            {
                if (this.Channels[i].Name == name)
                {
                    return Channels[i];
                }
            }

            return null;
        }
    }
}


using Irc.Forms;
using System.Collections.Generic;

namespace Irc.Irc
{
    class IrcServerInfo
    {
        private List<IrcServer> Servers = new List<IrcServer>();

        public void Select(string server, string channel)
        {
            this.GetServer(server).Select(channel);
        }

        public void UnSelect(string server, string channel)
        {
            this.GetServer(server).UnSelect(channel);
        }

        public void CreateServer(string server, string nick, Message message, UserList userlist)
        {
            this.Servers.Add(new IrcServer(server, nick, message, userlist));
        }

        public void SetTopic(string server, string channel, string topic)
        {
            this.GetServer(server).GetChannel(channel)?.SetTopic(topic);
        }

        public string GetTopic(string server, string channel)
        {
            return this.GetServer(server).GetChannel(channel).Topic;
        }

        public void AppendUser(string server, string channel, UserInfo info)
        {
            this.GetServer(server)?.GetChannel(channel)?.AppendUser(info);
        }

        public void RemoveUser(string server, string channel, string message)
        {
            this.GetServer(server).GetChannel(channel).RemoveUser(message);
        }

        public void UpdateNick(string server, string nick, string newNick)
        {
            this.GetServer(server).UpdateNick(nick, newNick);
        }

        public string GetNick(string server)
        {
            return this.GetServer(server).Nick;
        }

        public void RemoveServer(string server)
        {
            for(int i = 0; i < Servers.Count; i++)
            {
                if (Servers[i].Identify == server)
                {
                    Servers.RemoveAt(i);
                    return;
                }
            }
        }

        public string GetTopServer()
        {
            if (this.Servers.Count == 0)
                return null;
            return this.Servers[0].Identify;
        }

        public bool RemoveChannel(string server, string channel)
        {
            return this.GetServer(server).RemoveChannel(channel);
        }

        public string GetTopChannel(string server)
        {
            return this.GetServer(server).GetTopChannel();
        }

        public void Write(string server, string channel, string nick, string message)
        {
            this.GetServer(server).Write(channel, nick, message);
        }

        public bool IsChannelExists(string server, string channel)
        {
            return this.GetServer(server).IsChannelExists(channel);
        }

        public void CreateChannel(string server, string channel)
        {
            this.GetServer(server).CreateChannel(channel);
        }

        private IrcServer GetServer(string identify)
        {
            for(int i = 0; i < Servers.Count; i++)
            {
                if (Servers[i].Identify == identify)
                    return Servers[i];
            }
            return null;
        }
    }
}

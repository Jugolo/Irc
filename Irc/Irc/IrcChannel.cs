using Irc.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc
{
    class IrcChannel
    {
        public string Topic { get; private set; }
        public string Name { get; private set; }
        public bool IsSelected { get; private set; }
        public List<UserInfo> Users = new List<UserInfo>();
        public List<MessageData> Messages = new List<MessageData>();
        private Message Message;
        private UserList userlist;

        public IrcChannel(string name, Message message, UserList userlist)
        {
            this.Message = message;
            this.Name = name;
            this.userlist = userlist;
        }

        public void Select()
        {
            this.Message.Empty();
            this.userlist.Empty();
            for (int i = 0; i < this.Messages.Count; i++)
                this.Message.Write(this.Messages[i]);
            this.IsSelected = true;
            this.userlist.AppendUsers(this.Users);
        }

        public void UnSelect()
        {
            this.IsSelected = false;
        }

        public void SetTopic(string topic)
        {
            this.Topic = topic;
        }

        public void AppendUser(UserInfo user)
        {
            this.Users.Add(user);
        }

        public void RemoveUser(string nick)
        {
            for(int i = 0; i < this.Users.Count; i++)
            {
                if(this.Users[i].Nick == nick)
                {
                    this.Users.RemoveAt(i);
                    return;
                }
            }
        }

        public void UpdateNick(string nick, string newNick)
        {
            for(int i = 0; i < this.Users.Count; i++)
            {
                if(this.Users[i].Nick == nick)
                {
                    this.Users[i].Nick = newNick;
                    return;
                }
            }
        }

        public void Write(string nick, string message)
        {
            MessageData buffer = new MessageData(nick, message);
            this.Messages.Add(buffer);
            if (this.IsSelected)
            {
                this.Message.Write(buffer);
            }
        }
    }
}

using Irc.Irc.IS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc
{
    public class UserInfo : IComparable, NickGiven
    {
        public string Nick { get; set; }
        public bool Op { get; set; }
        public bool Voice { get; set; }

        public int CompareTo(object obj)
        {
            UserInfo f = this;
            UserInfo s = obj as UserInfo;
            string fn = (f.Op ? "@" : f.Voice ? "+" : "")+f.Nick;
            string sn = (s.Op ? "@" : s.Voice ? "+" : "") + s.Nick;
            return String.Compare(fn, sn);
        }

        public string GetNick()
        {
            return this.Nick;
        }
    }
}

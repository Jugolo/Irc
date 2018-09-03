using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc
{
    class IrcChannel
    {
        private Form1 main;
        private string name;
        private IrcConection connection;

        public IrcChannel(Form1 main, string name, IrcConection connection)
        {
            this.main = main;
            this.name = name;
            this.connection = connection;
        }
    }
}

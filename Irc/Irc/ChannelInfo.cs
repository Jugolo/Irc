using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc
{
    class ChannelInfo
    {
        public int CurrentLine = 0;
        public List<MessageParts> MessageParts = new List<MessageParts>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Values;

namespace Irc.Irc
{
    class IrcScriptMessage : Value
    {
        public override object ToPrimtiv()
        {
            return null;
        }

        public override string Type()
        {
            return "InternatinalIRCMessageValue";
        }

        public IrcMessage message { get;set;}

        public IrcScriptMessage(IrcMessage message)
        {
            this.message = message;
        }
    }
}

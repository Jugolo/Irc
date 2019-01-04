using Irc.Script;
using Irc.Script.Types;
using System.Collections.Generic;

namespace Irc.Irc
{
    class IrcScriptWhoIs : EcmaHeadObject
    {
        private WhoIsData data;

        public IrcScriptWhoIs(EcmaState state, WhoIsData data)
        {
            this.data = data;
            this.Put("nick", EcmaValue.String(data.Nick));
            this.Put("channels", EcmaValue.Object(EcmaUntil.ToArray(state, new List<object>(data.Channels))));
            this.Put("isAway", EcmaValue.Boolean(data.Away));
            this.Put("awayMessage", EcmaValue.String(data.AwayMessage));
        }
    }
}

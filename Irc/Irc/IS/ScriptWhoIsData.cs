using Irc.Script;
using Irc.Script.Types;
using Irc.Script.Types.Array;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc.IS
{
    class ScriptWhoIsData : EcmaHeadObject
    { 

        public ScriptWhoIsData(EcmaState state, WhoIsData data)
        {
            this.Put("isAway", EcmaValue.Boolean(data.Away));
            this.Put("awayMessage", EcmaValue.String(data.AwayMessage));
            this.Put("channels", data.Channels == null ? EcmaValue.Object(new ArrayIntstance(state, new EcmaValue[0])) : EcmaValue.Object(EcmaUntil.ToArray(state, new List<object>(data.Channels))));
            this.Put("nick", EcmaValue.String(data.Nick));
        }
    }
}

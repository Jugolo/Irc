using Irc.Script;
using Irc.Script.Types;

namespace Irc.Irc.IS
{
    class ScriptUserInfo : EcmaHeadObject
    {
        public ScriptUserInfo(UserInfo info)
        {
            this.Put("nick", EcmaValue.String(info.Nick));
            this.Put("op", EcmaValue.Boolean(info.Op));
            this.Put("voice", EcmaValue.Boolean(info.Voice));
        }
    }
}

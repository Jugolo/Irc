using Irc.Irc.IS;
using Irc.Script;
using Irc.Script.Types;

namespace Irc.Irc
{
    class IrcScriptMessage : EcmaHeadObject
    {
        public IrcScriptMessage(IrcMessage message)
        {
            this.Put("id", EcmaValue.String(message.Id));
            this.Put("nick", EcmaValue.String(message.Id));
            this.Put("channel", EcmaValue.String(message.ParamsMidle));
            this.Put("message", EcmaValue.String(message.ParamsTrailing));
        }
    }
}

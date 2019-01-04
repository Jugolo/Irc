using Irc.Script.Exceptions;

namespace Irc.Irc
{
    internal class IrcUntil
    {
        public static string GetChannel(IrcMessage message)
        {
            switch (message.Type)
            {
                case "PRIVMSG":
                    return message.ParamsMidle;
                case "JOIN":
                    return message.ParamsMidle;
                default:
                    throw new EcmaRuntimeException("Could not finde any channels in command: " + message.Type);
            }
        }

        internal static string ColoeredText(int text, int back, string message)
        {
            return '\x003'.ToString() + text + "," + back + message+"\x003";
        }

        internal static string Action(string message)
        {
            return '\x001'.ToString() + " " + message;
        }
    }
}
using System;
using System.Windows.Forms;
using torrent.Script.Error;

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
                    throw new ScriptRuntimeException("Could not finde any channels in command: " + message.Type);
            }
        }

        internal static string ColoeredText(int text, int back, string message)
        {
            return "\x003" + text + "," + back + message+"\x003";
        }
    }
}
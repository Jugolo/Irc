using Irc.Irc.IS;
using System.Text.RegularExpressions;

namespace Irc.Irc
{
    public class IrcMessage : NickGiven
    {
        public string Raw { get; set; }
        public string Prefix { get; set; }
        public string Type { get; set; }
        public string ParamsFull { get;set; }
        public bool IsClient { get; set; }
        public string Nick { get; set; }
        public string Id { get; set; }
        public string Mask { get; set; }
        public string ParamsMidle { get; set; }
        public string ParamsTrailing { get; set; }

        public static IrcMessage Parse(string line)
        { 

            IrcMessage message = new IrcMessage();
            message.Raw = line;
            if(line.IndexOf(":") == 0)
            {
                string user = line.Substring(1, line.IndexOf(" ") - 1);
                line = line.Substring(line.IndexOf(" ")+1);
                if(user.IndexOf("!") != -1)
                {
                    message.Nick = user.Substring(0, user.IndexOf("!"));
                    user = user.Substring(user.IndexOf("!") + 1);
                    if(user.IndexOf("@") != -1)
                    {
                        string[] data = user.Split('@');
                        message.Id = data[0];
                        message.Mask = data[1];
                    }
                    else
                    {
                        message.Id = user;
                    }
                }
                else
                {
                    message.Nick = user;
                }
            }
            else
            {
                message.Nick = "@@@SYSTEM@@@";
            }

            message.Type = line.Substring(0, line.IndexOf(" "));
            line = line.Substring(line.IndexOf(" "));

            if(line.IndexOf(" :") != -1)
            {
                message.ParamsMidle = line.Substring(0, line.IndexOf(" :")).Trim();
                message.ParamsTrailing = line.Substring(line.IndexOf(" :") + 2).TrimStart();
            }
            else
            {
                message.ParamsMidle = line.Trim();
            }

            return message;
        }

        public string GetNick()
        {
            return this.Nick;
        }
    }
}
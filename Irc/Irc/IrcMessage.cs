using System.Text.RegularExpressions;

namespace Irc.Irc
{
    public class IrcMessage
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
            line = line.Substring(line.IndexOf(" ") + 1);

            if(line.IndexOf(" :") != -1)
            {
                message.ParamsMidle = line.Substring(0, line.IndexOf(" :"));
                message.ParamsTrailing = line.Substring(line.IndexOf(" :") + 2);
            }
            else
            {
                message.ParamsMidle = line;
            }

            return message;

            string[] parts = line.Split(new char[] { ' ' }, 3);
            message.Raw = line;
            if (parts.Length == 3 && parts[0].IndexOf(":") == 0)
            {
                message.Prefix = parts[0];
                message.Type = parts[1];
                message.ParamsFull = parts[2];
            }
            else if (parts.Length == 2 && parts[0].IndexOf(":") != 0)
            {
                message.Type = parts[0];
                message.ParamsFull = parts[0];
            }
            else
            {
                return null;
            }

            Regex regex = new Regex("^:(.*)!(.*)@(.*)");
            if (message.Prefix != null && regex.IsMatch(message.Prefix))
            {
                Match match = regex.Match(message.Prefix);
                message.IsClient = true;
                message.Nick = match.Groups[1].Value;
                message.Id = match.Groups[2].Value;
                message.Mask = match.Groups[3].Value;
            }
            else
            {
                message.IsClient = false;
                string prefix = message.Prefix;
                if(prefix == null)
                    message.Nick = prefix.Substring(1);
            }

            parts = message.ParamsFull.Split(new char[] { ' ' }, 2);
            message.ParamsMidle = parts[0];
            if (parts.Length > 1)
                message.ParamsTrailing = parts[1].TrimStart().Substring(1);


            return message;
        }
    }
}
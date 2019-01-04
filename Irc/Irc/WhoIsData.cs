using Irc.Irc.IS;

namespace Irc.Irc
{
    public class WhoIsData : NickGiven, ChannelsGiven
    {
        public string Nick;
        public string[] Channels;
        public bool Away = false;
        public string AwayMessage;

        public string GetNick()
        {
            return this.Nick;
        }

        public string[] GetChannels()
        {
            return this.Channels;
        }
    }
}
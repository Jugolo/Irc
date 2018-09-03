using torrent.Script.Values;

namespace torrent.Script.Stack
{
    public class VariabelDatabaseValue
    {
        public Value Context { get; set; }
        public bool IsLock { get; set; }
        public bool IsGlobal { get; set; }

        public VariabelDatabaseValue()
        {
            this.IsLock = false;
            this.IsGlobal = false;
        }
    }
}
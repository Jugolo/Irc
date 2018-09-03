using torrent.Script.Values;

namespace torrent.Script
{
    public class Complication
    {
        private ComplicationType type;
        private Value value;

        public Complication(ComplicationType type, Value value)
        {
            this.type = type;
            this.value = value;
        }

        public Value GetValue()
        {
            return this.value;
        }

        public bool IsReturn()
        {
            return this.type == ComplicationType.Return;
        }

        public bool IsNormal()
        {
            return this.type == ComplicationType.Normal;
        }

        public bool IsContinue()
        {
            return this.type == ComplicationType.Continue;
        }

        public bool IsBreak()
        {
            return this.type == ComplicationType.Break;
        }
    }
}
using System;
using torrent.Script.Error;

namespace torrent.Script.Token
{
    public class TokenBuffer
    {
        private string type;
        private string context;

        public TokenBuffer(string type, string context)
        {
            this.type    = type;
            this.context = context;
        }

        public bool Is(string type)
        {
            return this.type == type;
        }

        public bool Is(string type, string context)
        {
            return this.type == type && this.context == context;
        }

        public string Context()
        {
            return this.context;
        }

        public string Type()
        {
            return this.type;
        }

        internal TokenBuffer Expect(string type, string context)
        {
            if (type != this.type || this.context != context)
                throw new ScriptParserException("Unexpected token detected " + this.context + "(" + this.type + "). Excpedted "+context+"("+type+")");
            return this;
        }

        internal TokenBuffer Expect(string type)
        {
            if (type != this.type)
                throw new ScriptParserException("Unexpected token detected " + this.context + "(" + this.type + ")");
            return this;
        }
    }
}
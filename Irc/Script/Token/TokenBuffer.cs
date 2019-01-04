using Irc.Script.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Token
{
    public class TokenBuffer
    {
        public TokenType Type { get; private set; }
        public string Context { get; private set; }
        public int LineStart { get; private set; }

        public TokenBuffer(TokenType type, string context, int lineStart)
        {
            this.Type = type;
            this.Context = context;
            this.LineStart = lineStart;
        }

        public bool Is(TokenType type, string[] tests)
        {
            if (this.Type != type)
                return false;

            for(int i = 0; i < tests.Length; i++)
            {
                if (this.Context == tests[i])
                    return true;
            }

            return false;
        }

        public bool Is(TokenType type, string context)
        {
            return this.Type == type && this.Context == context;
        }

        public bool Is(TokenType type)
        {
            return this.Type == type;
        }

        public bool IsNot(TokenType type, string context)
        {
            return !this.Is(type, context);
        }

        public bool IsNot(TokenType type)
        {
            return !this.Is(type);
        }

        public void Excepect(TokenType type)
        {
            if (!this.Is(type))
                throw new EcmaRuntimeException("Unexpected " + this.Type.ToString() + " detected expected " + type.ToString());
        }

        public void Excepect(TokenType type, string context)
        {
            if (!this.Is(type, context))
                throw new EcmaRuntimeException("Unexpected " + this.Type.ToString() + "(" + this.Context + ") expected " + type.ToString() + "(" + context + ") on line: "+this.LineStart);
        }
    }
}

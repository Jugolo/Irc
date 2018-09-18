using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;

namespace torrent.Script.Token
{
    public class Tokenizer
    {
        private InputReader reader;
        private TokenBuffer buffer;

        public Tokenizer(InputReader reader)
        {
            this.reader = reader;
            this.Next();
        }

        public TokenBuffer Current()
        {
            return this.buffer;
        }

        public TokenBuffer Next()
        {
            return this.buffer = this.Generate();
        }

        private TokenBuffer Generate() {
            int c = this.GarbageCollect();

            if(c == -1)
            {
                return new TokenBuffer("EOS", "End of string");
            }

            if (this.IsIdentifyStart(c))
                return this.GetIdentify(c);

            if (this.IsNumber(c))
                return this.GetNumber(c);

            if (c == '"' || c == '\'')
                return this.GetString(c);

            return this.Punctor(c);
        }

        private TokenBuffer GetNumber(int c)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append((char)c);
            this.GetNumberPart(builder);
            if(this.reader.Peek() == '.')
            {
                builder.Append(".");
                this.GetNumberPart(builder);
            }
            return new TokenBuffer("number", builder.ToString());
        }

        private void GetNumberPart(StringBuilder builder)
        {
            while (this.IsNumber(this.reader.Peek()))
                builder.Append((char)this.reader.Read());
        }

        private TokenBuffer GetString(int end)
        {
            StringBuilder builder = new StringBuilder();
            while (!this.reader.IsEnd())
            {
                int c = this.reader.Read();
                if (c == end)
                    return new TokenBuffer("string", builder.ToString());
                builder.Append((char)c);
            }

            throw new ScriptParserException("Missing end of string char: " + ((char)end));
        }

        private TokenBuffer GetIdentify(int c)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append((char)c);
            while (this.IsIdentifyPart(this.reader.Peek()))
                builder.Append((char)this.reader.Read());
            string name = builder.ToString();
            switch (name)
            {
                case "function":
                case "while":
                case "if":
                case "elseif":
                case "else":
                case "return":
                case "as":
                case "foreach":
                case "for":
                case "global":
                case "delete":
                    return new TokenBuffer("keyword", name);
                case "true":
                case "false":
                    return new TokenBuffer("bool", name);
                case "null":
                    return new TokenBuffer("null", "null");
            }
            return new TokenBuffer("identify", name);
        }

        private TokenBuffer Punctor(int c)
        {
            switch (c)
            {
                case '(':
                    return new TokenBuffer("punctor", "(");
                case ')':
                    return new TokenBuffer("punctor", ")");
                case '{':
                    return new TokenBuffer("punctor", "{");
                case '}':
                    return new TokenBuffer("punctor", "}");
                case ';':
                    return new TokenBuffer("punctor", ";");
                case '=':
                    if(this.reader.Peek() == '=')
                    {
                        this.reader.Read();
                        return new TokenBuffer("punctor", "==");
                    }
                    return new TokenBuffer("punctor", "=");
                case '!':
                    if(this.reader.Peek() == '=')
                    {
                        this.reader.Read();
                        return new TokenBuffer("punctor", "!=");
                    }
                    return new TokenBuffer("punctor", "!");
                case ',':
                    return new TokenBuffer("punctor", ",");
                case '+':
                    if(this.reader.Peek() == '+')
                    {
                        this.reader.Read();
                        return new TokenBuffer("punctor", "++");
                    }
                    return new TokenBuffer("punctor", "+");
                case '-':
                    if(this.reader.Peek() == '-')
                    {
                        this.reader.Read();
                        return new TokenBuffer("punctor", "--");
                    }
                    return new TokenBuffer("punctor", "-");
                case '[':
                    return new TokenBuffer("punctor", "[");
                case ']':
                    return new TokenBuffer("punctor", "]");
                case ':':
                    return new TokenBuffer("punctor", ":");
                case '<':
                    if(this.reader.Peek() == '=')
                    {
                        this.reader.Read();
                        return new TokenBuffer("punctor", "<=");
                    }
                    return new TokenBuffer("punctor", "<");
                case '>':
                    if(this.reader.Peek() == '=')
                    {
                        this.reader.Read();
                        return new TokenBuffer("punctor", ">=");
                    }
                    return new TokenBuffer("punctor", ">");
                case '&':
                    if(this.reader.Peek() == '&')
                    {
                        this.reader.Read();
                        return new TokenBuffer("punctor", "&&");
                    }
                    return new TokenBuffer("punctor", "&");
            }

            throw new ScriptParserException("Unknown char detected: " + (((char)c).ToString()));
        }

        private bool IsNumber(int c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsIdentifyPart(int c)
        {
            return this.IsIdentifyStart(c) || this.IsNumber(c);
        }

        private bool IsIdentifyStart(int c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c == '$' || c == '_';
        }

        private int GarbageCollect()
        {
            while (!this.reader.IsEnd())
            {
                int i = this.reader.Read();

                if(i == ' ' || i == '\n' || i == '\r')
                {
                    continue;
                }

                return i;
            }
            return -1;
        }
    }
}

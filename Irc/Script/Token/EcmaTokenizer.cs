using Irc.Script.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script.Token
{
    public class EcmaTokenizer
    {
        public EcmaReader Reader { get; private set; }
        private TokenBuffer buffer;
        private int LastLineStart = -1;
        public List<int> Lines = new List<int>();

        public EcmaTokenizer(TextReader reader)
        {
            this.Reader = new EcmaReader(reader);
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

        private TokenBuffer Generate()
        {
            int c = this.GarbageCollector();

            if(c == -1)
            {
                return new TokenBuffer(TokenType.EOS, "end of script", this.LastLineStart);
            }

            if (EcmaChar.IsVariabelStart(c))
                return GetIdentify(c);

            if (EcmaChar.IsDigit(c))
                return GetNumber(c);

            if (c == '\'' || c == '"')
                return GetString(c);

            return GetPunctor(c);
        }

        private TokenBuffer GetNumber(int c)
        {
            if(c == '0')
            {
                int buf = this.Reader.Peek();
                if(buf == 'x' || buf == 'X')
                {
                    this.Reader.Read();
                    if (!EcmaChar.IsHexDigit(this.Reader.Peek()))
                        throw new EcmaRuntimeException("After 0X there must be a hex digit");
                    return new TokenBuffer(TokenType.Number, Convert.ToUInt32(((char)this.Reader.Read()).ToString(), 16).ToString(), this.LastLineStart);
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.Append((char)c);
            GetInt(builder);
            if(this.Reader.Peek() == '.')
            {
                this.Reader.Read();
                builder.Append(".");
                this.GetInt(builder);
            }
            if(this.Reader.Peek() == 'e' || this.Reader.Peek() == 'E')
            {
                builder.Append((char)this.Reader.Read());
            }

            return new TokenBuffer(TokenType.Number, builder.ToString(), this.LastLineStart);
        }

        private void GetInt(StringBuilder builder)
        {
            while (EcmaChar.IsDigit(this.Reader.Peek()))
                builder.Append((char)this.Reader.Read());
        }

        private TokenBuffer GetString(int stop)
        {
            int c;
            StringBuilder str = new StringBuilder();

            while (true)
            {
                c = this.Reader.Read();
                if (EcmaChar.IsLineTerminator(c))
                    throw new EcmaRuntimeException("A string cant containe a line terminator. Use \\n instance");

                if(c == stop)
                    return new TokenBuffer(TokenType.String, str.ToString(), this.LastLineStart);


                if(c == '\\')
                {
                    c = this.Reader.Read();
                    switch (c)
                    {
                        case '\'':
                            str.Append("'");
                            break;
                        case '"':
                            str.Append("\"");
                            break;
                        case '\\':
                            str.Append('\\');
                            break;
                        case 'b':
                            str.Append('\u0008');
                            break;
                        case 't':
                            str.Append('\u0009');
                            break;
                        case 'n':
                            str.Append('\u000A');
                            break;
                        case 'f':
                            str.Append("\u000C");
                            break;
                        case 'r':
                            str.Append('\u000D');
                            break;
                        case 'x':
                            str.Append(GetStringHex());
                            break;
                        case 'u':
                            str.Append(GetStringUnicode());
                            break;
                        default:
                            if (EcmaChar.IsOctalDigit(c))
                            {
                                str.Append(GetStringOctal(c));
                            }
                            str.Append((char)c);
                            break;
                    }
                    continue;
                }

                str.Append((char)c);
            }
        }

        private char GetStringUnicode()
        {
            StringBuilder builder = new StringBuilder();
            if (!EcmaChar.IsHexDigit(this.Reader.Peek()))
                throw new EcmaRuntimeException("After \\u there must be a hex digit");
            builder.Append((char)this.Reader.Read());

            if (!EcmaChar.IsHexDigit(this.Reader.Peek()))
                throw new EcmaRuntimeException("After \\u[hex] there must be a hex digit");
            builder.Append((char)this.Reader.Read());

            if (!EcmaChar.IsHexDigit(this.Reader.Peek()))
                throw new EcmaRuntimeException("After \\u[hex][hex] there must be a hex digit");
            builder.Append((char)this.Reader.Read());

            if (!EcmaChar.IsHexDigit(this.Reader.Peek()))
                throw new EcmaRuntimeException("After \\u[hex][hex][hex] there must be a hex digit");
            builder.Append((char)this.Reader.Read());

            return (char)Convert.ToUInt32(builder.ToString(), 16);
        }

        private char GetStringHex()
        {
            StringBuilder builder = new StringBuilder();

            if (!EcmaChar.IsHexDigit(this.Reader.Peek()))
                throw new EcmaRuntimeException("After \\x in string ther must be hex digit");

            builder.Append((char)this.Reader.Read());

            if (!EcmaChar.IsHexDigit(this.Reader.Peek()))
                throw new EcmaRuntimeException("After \\x[hex] there must be a hex digit");
            builder.Append((char)this.Reader.Read());

            return (char)Convert.ToUInt32(builder.ToString(), 16);
        }

        private char GetStringOctal(int c)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append((char)c);

            if (EcmaChar.IsOctalDigit(this.Reader.Peek()))
            {
                builder.Append((char)this.Reader.Read());
                if(c >= '0' && c <= '3' && EcmaChar.IsOctalDigit(this.Reader.Peek()))
                {
                    builder.Append((char)this.Reader.Read());
                }
            }

            return (char)Convert.ToInt32(builder.ToString(), 8);
        }

        private TokenBuffer GetIdentify(int c)
        {
            StringBuilder identify = new StringBuilder();
            identify.Append((char)c);
            while (EcmaChar.IsVariabelPart(this.Reader.Peek()))
                identify.Append((char)this.Reader.Read());

            string i = identify.ToString();

            switch (i)
            {
                case "break":
                case "for":
                case "new":
                case "var":
                case "continue":
                case "function":
                case "return":
                case "void":
                case "delete":
                case "if":
                case "this":
                case "while":
                case "else":
                case "in":
                case "typeof":
                case "with":
                    return new TokenBuffer(TokenType.Keyword, i, this.LastLineStart);
                case "null":
                    return new TokenBuffer(TokenType.Null, "null", this.LastLineStart);
                case "true":
                case "false":
                    return new TokenBuffer(TokenType.Bool, i, this.LastLineStart);
            }

            return new TokenBuffer(TokenType.Identify, i, this.LastLineStart);
        }

        private TokenBuffer GetPunctor(int c)
        {
            string sign = null;

            if(c == '=')
            {
                sign = "=";
                if(this.Reader.Peek() == '=')
                {
                    this.Reader.Read();
                    sign += "=";
                }
            }else if(c == '>')
            {
                sign = ">";
                if(this.Reader.Peek() == '=')
                {
                    this.Reader.Read();
                    sign += "=";
                }else if(this.Reader.Peek() == '>')
                {
                    this.Reader.Read();
                    sign += ">";
                    if(this.Reader.Peek() == '>')
                    {
                        this.Reader.Read();
                        sign += ">";
                        if (this.Reader.Peek() == '=')
                        {
                            this.Reader.Read();
                            sign += "=";
                        }
                    }else if(this.Reader.Peek() == '=')
                    {
                        this.Reader.Read();
                        sign += "=";
                    }
                }
            }else if(c == '<')
            {
                sign = "<";
                if(this.Reader.Peek() == '=')
                {
                    this.Reader.Read();
                    sign += "=";
                }else if(this.Reader.Peek() == '<')
                {
                    this.Reader.Read();
                    sign += "<";
                    if(this.Reader.Peek() == '=')
                    {
                        this.Reader.Read();
                        sign = "=";
                    }
                }
            }else if(c == ',')
            {
                sign = ",";
            }else if(c == '!')
            {
                sign = "!";
                if(this.Reader.Peek() == '=')
                {
                    this.Reader.Read();
                    sign += "=";
                }
            }else if(c == '~')
            {
                sign = "~";
            }else if(c == '?')
            {
                sign = "?";
            }else if(c == ':')
            {
                sign = ":";
            }else if(c == '.')
            {
                sign = ".";
            }else if(c == '+')
            {
                sign = "+";
                if(this.Reader.Peek() == '+')
                {
                    sign += "+";
                    this.Reader.Read();
                }else if(this.Reader.Peek() == '=')
                {
                    sign += "=";
                    this.Reader.Read();
                }
            }else if(c == '-')
            {
                sign = "-";
                if(this.Reader.Peek() == '-')
                {
                    this.Reader.Read();
                    sign += "-";
                }else if(this.Reader.Peek() == '=')
                {
                    this.Reader.Read();
                    sign += "=";
                }
            }else if(c == '*')
            {
                sign = "*";
                if(this.Reader.Peek() == '=')
                {
                    sign += "=";
                    this.Reader.Read();
                }
            }else if(c == '/')
            {
                sign = "/";
                if(this.Reader.Peek() == '=')
                {
                    sign += "=";
                }
            }else if(c == '&')
            {
                sign = "&";
                if(this.Reader.Peek() == '&')
                {
                    sign += "&";
                    this.Reader.Read();
                }else if(this.Reader.Peek() == '=')
                {
                    sign += "=";
                    this.Reader.Read();
                }
            }else if(c == '|')
            {
                sign = "|";
                if(this.Reader.Peek() == '|')
                {
                    sign += "|";
                    this.Reader.Read();
                }else if(this.Reader.Peek() == '=')
                {
                    sign += "=";
                    this.Reader.Read();
                }
            }else if(c == '^')
            {
                sign = "^";
                if(this.Reader.Peek() == '=')
                {
                    sign += "=";
                    this.Reader.Read();
                }
            }else if(c == '%')
            {
                sign = "%";
                if(this.Reader.Peek() == '=')
                {
                    sign = "=";
                    this.Reader.Read();
                }
            }else if(c == '(')
            {
                sign = "(";
            }else if(c == ')')
            {
                sign = ")";
            }else if(c == '{')
            {
                sign = "{";
            }else if(c == '}')
            {
                sign = "}";
            }else if(c == '[')
            {
                sign = "[";
            }else if(c == ']')
            {
                sign = "]";
            }else if(c == ';')
            {
                sign = ";";
            }

            if(sign == null)
                throw new EcmaRuntimeException("Unknwon char detected: " + (char)c + "(" + c + ")");

            return new TokenBuffer(TokenType.Punctor, sign, this.LastLineStart);
        }

        private int GarbageCollector()
        {
            int c = this.Reader.Read();
            this.LastLineStart = this.Reader.Line;
            this.Lines.Add(this.Reader.Line);

            if (EcmaChar.IsWhiteSpace(c))
                return GarbageCollector();
            if (EcmaChar.IsLineTerminator(c))
                return GarbageCollector();

            if(c == '/')
            {
                int b = this.Reader.Peek();
                if(b == '/')
                {
                    while (!EcmaChar.IsLineTerminator(this.Reader.Read()))
                        ;
                    return GarbageCollector();
                }

                if(b == '*')
                {
                    this.Reader.Read();
                    while (!(this.Reader.Read() == '*' && this.Reader.Peek() == '/'))
                        ;
                    this.Reader.Read();
                    return GarbageCollector();
                }
            }

            return c;
        }
    }
}

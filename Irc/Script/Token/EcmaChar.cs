using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Token
{
    class EcmaChar
    {
        public static bool IsWhiteSpace(int c)
        {
            return c == '\u0009' || c == '\u000B' || c == '\u000C' || c == '\u0020';
        }

        public static bool IsLineTerminator(int c)
        {
            return c == '\u000A' || c == '\u000D';
        }

        public static bool IsVariabelStart(int c)
        {
            return c >= 'a' && c <= 'z'
                || c >= 'A' && c <= 'Z'
                || c == '_' || c == '$';
        }

        public static bool IsVariabelPart(int c)
        {
            return IsVariabelStart(c) || IsDigit(c);
        }

        public static bool IsDigit(int c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsOctalDigit(int c)
        {
            return c >= '0' && c <= '7';
        }

        public static bool IsHexDigit(int c)
        {
            return IsDigit(c) ||
                   c >= 'a' && c <= 'f'
                || c >= 'A' && c <= 'F';
        }
    }
}

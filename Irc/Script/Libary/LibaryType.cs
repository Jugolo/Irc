using System.Collections.Generic;
using torrent.Script.Values;

namespace torrent.Script.Libary
{
    internal class LibaryType
    {
        public static Value GetType(Value[] args)
        {
            return new StringValue(args[0].Type());
        }

        public static Value GetNumric(Value[] args)
        {
            char c = args[0].toString().ToCharArray()[0];
            return new BoolValue(c >= '0' && c <= '9');
        }

        public static Value GetToNumber(Value[] args)
        {
            double result;
            if(double.TryParse(args[0].toString(), out result))
            {
                return new NumberValue(result);
            }
            return new NumberValue(-1);
        }
    }
}
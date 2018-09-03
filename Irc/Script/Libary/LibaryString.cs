using System.Collections.Generic;
using torrent.Script.Values;

namespace torrent.Script.Libary
{
    internal class LibaryString
    {
        public static Value Length(Value[] args)
        {
            return new NumberValue(args[0].toString().Length);
        }

        public static Value CharArray(Value[] args)
        {
            List<Value> result = new List<Value>();
            char[] c = args[0].toString().ToCharArray();
            for (int i = 0; i < c.Length; i++)
                result.Add(new StringValue(c[i].ToString()));
            return new ArrayValue(result);
        }
    }
}
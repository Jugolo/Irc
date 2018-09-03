using System.Collections.Generic;
using torrent.Script.Values;

namespace torrent.Script.Libary
{
    internal class LibaryArray
    {
        public static Value GetCount(Value[] args)
        {
            return new NumberValue(args[0].ToArray().Count);
        }
    }
}
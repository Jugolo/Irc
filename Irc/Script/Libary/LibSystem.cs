using System;
using System.Collections.Generic;
using System.Windows.Forms;
using torrent.Script.Values;

namespace torrent.Script.Libary
{
    internal class LibSystem
    {
        private static Random rand = new Random();

        public static Value Alert(Value[] args)
        {
            MessageBox.Show(args[0].toString());
            return new NullValue();
        }

        public static Value Rand(Value[] args)
        {
            return new NumberValue(rand.Next((int)args[0].ToNumber(), (int)args[1].ToNumber()));
        }
    }
}
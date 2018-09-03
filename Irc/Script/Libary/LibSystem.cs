using System.Collections.Generic;
using System.Windows.Forms;
using torrent.Script.Values;

namespace torrent.Script.Libary
{
    internal class LibSystem
    {
        public static Value Alert(Value[] args)
        {
            MessageBox.Show(args[0].toString());
            return new NullValue();
        }
    }
}
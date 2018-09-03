using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using torrent.Script.Values;

namespace torrent.Script.Libary
{
    internal class LibaryFile
    {
        public static Value GetExists(Value[] args)
        {
            return new BoolValue(File.Exists(args[0].toString()));
        }

        public static Value GetCreate(Value[] args)
        {
            string filename = args[0].toString();
            if (!File.Exists(filename))
            {
                File.Create(filename).Close();
                return new BoolValue(true);
            }
            return new BoolValue(false);
        }

        public static Value GetGetContents(Value[] args)
        {
            string filename = args[0].toString();
            if (File.Exists(filename))
            {
                return new StringValue(File.ReadAllText(filename));
            }
            return new NullValue();
        }

        public static Value GetPutContents(Value[] args)
        {
            string filename = args[0].toString();
            if (!File.Exists(filename))
                return new BoolValue(false);
            
            File.WriteAllText(filename, args[1].toString());
            return new BoolValue(true);
        }
    }
}
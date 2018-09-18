using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Values;

namespace Irc.Script.Libary
{
    class LibHash
    {
        public static Value Sha1(Value[] args)
        {
            SHA1Managed sha1 = new SHA1Managed();
            byte[] bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(args[0].toString()));
            StringBuilder builder = new StringBuilder(bytes.Length * 2);
            for(int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("X2"));
            }
            sha1.Dispose();
            return new StringValue(builder.ToString());
        }
    }
}

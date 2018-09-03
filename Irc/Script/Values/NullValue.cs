using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Values
{
    class NullValue : Value
    {
        public override string Type()
        {
            return "null";
        }

        public override object ToPrimtiv()
        {
            return null;
        }

        public override bool ToBool()
        {
            return false;
        }

        public override string toString()
        {
            return "";
        }
    }
}

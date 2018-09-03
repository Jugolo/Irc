using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Values
{
    class StringValue : Value
    {
        private string value;

        public StringValue(string value)
        {
            this.value = value;
        }

        public override string Type()
        {
            return "string";
        }

        public override object ToPrimtiv()
        {
            return this.value;
        }

        public override string toString()
        {
            return this.value;
        }

        public override bool ToBool()
        {
            return this.value.Length != 0;
        }
    }
}

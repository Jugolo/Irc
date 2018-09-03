using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Values
{
    class BoolValue : Value
    {
        private bool value;

        public BoolValue(bool value)
        {
            this.value = value;
        }

        public override string Type()
        {
            return "bool";
        }

        public override object ToPrimtiv()
        {
            return this.value;
        }

        public override bool ToBool()
        {
            return this.value;
        }
    }
}

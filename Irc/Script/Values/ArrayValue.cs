using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Values
{
    class ArrayValue : Value
    {
        private List<Value> value;

        public ArrayValue(List<Value> value)
        {
            this.value = value;
        }

        public override string Type()
        {
            return "array";
        }

        public override object ToPrimtiv()
        {
            return this.value;
        }

        public override List<Value> ToArray()
        {
            return this.value;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Values
{
    class NamedArrayValue : Value
    {
        private Dictionary<string, Value> value;

        public NamedArrayValue(Dictionary<string, Value> value)
        {
            this.value = value;
        }

        public override string Type()
        {
            return "namedArray";
        }

        public override object ToPrimtiv()
        {
            return this.value;
        }

        public override Dictionary<string, Value> ToNamedArray()
        {
            return this.value;
        }
    }
}

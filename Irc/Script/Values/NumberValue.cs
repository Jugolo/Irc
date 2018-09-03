using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Values
{
    class NumberValue : Value
    {
        private double value;

        public NumberValue(double value)
        {
            this.value = value;
        }

        public override string Type()
        {
            return "number";
        }

        public override object ToPrimtiv()
        {
            return this.value;
        }

        public override double ToNumber()
        {
            return this.value;
        }

        public override string toString()
        {
            if (this.value == 0)
                return "0";
            return this.value.ToString();
        }
    }
}

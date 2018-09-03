using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Refrences;

namespace torrent.Script.Values
{
    class RefrenceValue : Value
    {
        private Refrence refrence;

        public RefrenceValue(Refrence refrence)
        {
            this.refrence = refrence;
        }

        public override string Type()
        {
            return this.refrence.Get().Type();
        }

        public override object ToPrimtiv()
        {
            return this.refrence.Get().ToPrimtiv();
        }

        public override FunctionInstance ToFunction()
        {
            return this.refrence.Get().ToFunction();
        }

        public override Refrence ToRefrence()
        {
            return this.refrence;
        }

        public override string toString()
        {
            return this.refrence.Get().toString();
        }

        public override double ToNumber()
        {
            return this.refrence.Get().ToNumber();
        }

        public override bool ToBool()
        {
            return this.refrence.Get().ToBool();
        }

        public override List<Value> ToArray()
        {
            return this.refrence.Get().ToArray();
        }
    }
}

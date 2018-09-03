using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Refrences;

namespace torrent.Script.Values
{
    public abstract class Value
    {
        public abstract string Type();
        public abstract object ToPrimtiv();

        public virtual FunctionInstance ToFunction()
        {
            throw new ScriptRuntimeException("Cant convert " + this.Type() + " to function");
        }

        public virtual string toString()
        {
            throw new ScriptRuntimeException("Cant convert " + this.Type() + " to string");
        }

        public virtual Refrence ToRefrence()
        {
            throw new ScriptRuntimeException("Cant convert " + this.Type() + " to refrence");
        }

        public virtual bool ToBool()
        {
            throw new ScriptRuntimeException("Cant convert " + this.Type() + " to bool");
        }

        public virtual double ToNumber()
        {
            throw new ScriptRuntimeException("Cant convert " + this.Type() + " to number");
        }

        public virtual List<Value> ToArray()
        {
            throw new ScriptRuntimeException("Cant convert " + this.Type() + " to array");
        }

        public virtual Dictionary<string, Value> ToNamedArray()
        {
            throw new ScriptRuntimeException("Cant convert " + this.Type() + " to named array");
        }
    }
}

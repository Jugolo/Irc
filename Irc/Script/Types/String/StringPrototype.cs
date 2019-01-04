using Irc.Script.Exceptions;
using Irc.Script.Types.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.String
{
    public class StringPrototype : EcmaHeadObject
    {
        private StringConstructor constructor;
        private EcmaState State;

        public StringPrototype(StringConstructor constructor)
        {
            this.constructor = constructor;
            this.State = constructor.State;
            this.Class = "String";
            this.Prototype = constructor.State.Object;

            Put("constructor", EcmaValue.Object(constructor));
            Put("toString", EcmaValue.Object(new NativeFunctionInstance(0, State, EcmaToString)));
            Put("valueOf", EcmaValue.Object(new NativeFunctionInstance(0, State, ValueOf)));
            Put("charAt", EcmaValue.Object(new NativeFunctionInstance(1, State, CharAt)));
            Put("charCodeAt", EcmaValue.Object(new NativeFunctionInstance(1, State, CharCodeAt)));
            Put("indexOf", EcmaValue.Object(new NativeFunctionInstance(1, State, IndexOf)));
        }

        public EcmaValue IndexOf(EcmaHeadObject obj, EcmaValue[] arg)
        {
            string str = EcmaValue.Object(obj).ToString(State);
            string s = arg[0].ToString(State);
            int pos = arg.Length == 1 ? 0 : arg[1].ToInt32(State);

            return EcmaValue.Number(str.IndexOf(s, pos));
        }

        public EcmaValue CharCodeAt(EcmaHeadObject obj, EcmaValue[] arg)
        {
            string str = EcmaValue.Object(obj).ToString(State);
            int pos = arg[0].ToInt32(State);
            int length = str.Length;

            if (pos < 0 || pos > length)
                return EcmaValue.Number(Double.NaN);

            return EcmaValue.Number((int)str.ToCharArray()[pos]);
        }

        public EcmaValue CharAt(EcmaHeadObject obj, EcmaValue[] arg)
        {
            string str = obj == null ? "" : obj.Value.ToString(State);
            double pos = arg[0].ToInteger(State);
            if (pos < 0 || pos >= str.Length)
                return EcmaValue.String("");
            return EcmaValue.String(str.ToCharArray()[(int)pos].ToString());
        }

        public EcmaValue EcmaToString(EcmaHeadObject obj, EcmaValue[] args)
        {
            if(!(obj is StringInstance))
            {
                throw new EcmaRuntimeException("String.prototype.toString can only be called when it is a part of string object");
            }

            return obj.Value;
        }

        public EcmaValue ValueOf(EcmaHeadObject obj, EcmaValue[] args)
        {
            if (!(obj is StringInstance))
            {
                throw new EcmaRuntimeException("String.prototype.valueOf can only be called when it is a part of string object");
            }

            return obj.Value;
        }
    }
}

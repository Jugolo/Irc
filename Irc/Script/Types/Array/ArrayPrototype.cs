using Irc.Script.Exceptions;
using Irc.Script.Types.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Array
{
    public class ArrayPrototype : EcmaHeadObject
    {
        private EcmaState state;

        public ArrayPrototype(ArrayConstructor constructor, EcmaState state)
        {
            this.state = state;
            this.Prototype = state.Object;
            this.Put("constructor", EcmaValue.Object(constructor));
            this.Put("toString", EcmaValue.Object(new NativeFunctionInstance(0, state, EcmaToString)));
            this.Put("join", EcmaValue.Object(new NativeFunctionInstance(0, state, Join)));
            this.Put("reverse", EcmaValue.Object(new NativeFunctionInstance(0, state, Reverse)));
            this.Put("sort", EcmaValue.Object(new NativeFunctionInstance(1, state, Sort)));
        }

        private EcmaValue Sort(EcmaHeadObject obj, EcmaValue[] arg)
        {
            throw new EcmaRuntimeException("Dont supported Array.prototype.sort yet!");
        }

        private EcmaValue Reverse(EcmaHeadObject obj, EcmaValue[] arg)
        {
            uint length = obj.Get("length").ToUint32(state);
            uint mid = (uint)Math.Floor((double)length / 2);
            uint k = 0;
            while(k != mid)
            {
                uint l = length - k - 1;
                if(obj.HasProperty(k.ToString()))
                {
                    if (obj.HasProperty(l.ToString()))
                    {
                        EcmaValue a = obj.Get(k.ToString());
                        obj.Put(k.ToString(), obj.Get(l.ToString()));
                        obj.Put(l.ToString(), a);
                    }
                    else
                    {
                        obj.Put(l.ToString(), obj.Get(k.ToString()));
                        obj.Delete(k.ToString());
                    }
                }
                else
                {
                    if (obj.HasProperty(l.ToString()))
                    {
                        obj.Put(k.ToString(), obj.Get(l.ToString()));
                        obj.Delete(l.ToString());
                    }
                    else
                    {
                        obj.Delete(l.ToString());
                        obj.Delete(k.ToString());
                    }
                }
                k++;
            }
            return EcmaValue.Object(obj);
        }

        private EcmaValue EcmaToString(EcmaHeadObject obj, EcmaValue[] arg)
        {
            return Join(obj, new EcmaValue[0]);
        }

        private EcmaValue Join(EcmaHeadObject obj, EcmaValue[] arg)
        {
            string seperator = arg.Length == 0 ? "," : arg[0].ToString(state);
            uint length = obj.Get("length").ToUint32(state);
            if (length == 0)
                return EcmaValue.String("");

            EcmaValue value = obj.Get("0");
            string R = value.IsUndefined() || value.IsNull() ? "" : value.ToString(state);
            for(uint k = 1; k < length; k++)
            {
                value = obj.Get(k.ToString());
                R += seperator + (value.IsUndefined() || value.IsNull() ? "" : value.ToString(state));
            }

            return EcmaValue.String(R);
        }
    }
}

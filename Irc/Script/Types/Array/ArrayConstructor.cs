using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Array
{
    public class ArrayConstructor : EcmaHeadObject, ICallable, IConstruct
    {
        public ArrayPrototype prototype;
        public EcmaState State;

        public ArrayConstructor(EcmaState state)
        {
            this.State = state;
            state.Array = new ArrayPrototype(this, state);
            this.Put("prototype", EcmaValue.Object(state.Array));
            this.Put("length", EcmaValue.Number(1));
            this.Property["prototype"].DontDelete = true;
            this.Property["prototype"].ReadOnly = true;
            this.Property["prototype"].DontEnum = true;
        }

        public EcmaValue Call(EcmaHeadObject obj, EcmaValue[] arg)
        {
            return this.Construct(arg);
        }

        public EcmaValue Construct(EcmaValue[] arg)
        {
            return EcmaValue.Object(new ArrayIntstance(this.State, arg));
        }
    }
}

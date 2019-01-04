using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Object
{
    public class ObjectConstructor : EcmaHeadObject, ICallable, IConstruct
    {
        public EcmaState State;

        public ObjectConstructor(EcmaState state)
        {
            this.State = state;
            ObjectPrototype proto = new ObjectPrototype(this);
            Put("length", EcmaValue.Number(0));
            Put("prototype", EcmaValue.Object(proto));
            Property["prototype"].DontDelete = true;
            Property["prototype"].DontEnum = true;
            Property["prototype"].ReadOnly = true;

            State.Object = proto;
        }

        public EcmaValue Call(EcmaHeadObject obj, EcmaValue[] arg)
        {
            return Construct(arg);
        }

        public EcmaValue Construct(EcmaValue[] arg)
        {
            if(arg.Length > 0)
            {
                if (arg[0].IsObject())
                    return arg[0];

                switch (arg[0].Type())
                {
                    case EcmaValueType.String:
                    case EcmaValueType.Number:
                    case EcmaValueType.Boolean:
                        return EcmaValue.Object(arg[0].ToObject(State));
                }
            }
            return EcmaValue.Object(new ObjectInstance(this.State, arg));
        }
    }
}

using Irc.Script.Types.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Object
{
    public class ObjectPrototype : EcmaHeadObject
    {
        private ObjectConstructor constructor;

        public ObjectPrototype(ObjectConstructor constructor)
        {
            this.constructor = constructor;
            Put("constructor", EcmaValue.Object(constructor));
            Put("toString", EcmaValue.Object(new NativeFunctionInstance(0, constructor.State, EcmaToString)));
            Put("valueOf", EcmaValue.Object(new NativeFunctionInstance(0, constructor.State, ValueOf)));
        }

        public EcmaValue ValueOf(EcmaHeadObject obj, EcmaValue[] args)
        {
            return EcmaValue.Object(obj);
        }

        public EcmaValue EcmaToString(EcmaHeadObject obj, EcmaValue[] args)
        {
            return EcmaValue.String("[object " + obj.Class + "]");
        }
    }
}

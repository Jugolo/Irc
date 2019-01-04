using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Function
{
    class NativeFunctionInstance : FunctionInstance, ICallable
    {
        public delegate EcmaValue _C(EcmaHeadObject obj, EcmaValue[] arg);
        private _C _Call;

        public NativeFunctionInstance(int length, EcmaState state, _C method) : base(state, length)
        {
            this._Call = method;
        }

        public override EcmaValue Call(EcmaHeadObject obj, EcmaValue[] arg)
        {
            EcmaValue value = this._Call(obj, arg);
            if (value == null)
                return EcmaValue.Null();
            return value;
        }
    }
}

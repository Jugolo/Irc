using Irc.Script.Types.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script.Types.String
{
    public class StringConstructor : EcmaHeadObject, ICallable, IConstruct
    {
        public EcmaState State;
        public StringPrototype prototype;

        public StringConstructor(EcmaState state)
        {
            this.State = state;
            state.String = this;
            this.Prototype = state.Function;
            this.prototype = new StringPrototype(this);
            Put("length", EcmaValue.Number(0));
            Put("prototype", EcmaValue.Object(this.prototype));
            Put("fromCharCode", EcmaValue.Object(new NativeFunctionInstance(0, State, FromCharCode)));

            Property["prototype"].DontDelete = true;
            Property["prototype"].DontEnum = true;
            Property["prototype"].ReadOnly = true;
        }

        public EcmaValue FromCharCode(EcmaHeadObject obj, EcmaValue[] args)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
                builder.Append((char)args[i].ToInt32(State));
            return EcmaValue.String(builder.ToString());
        }

        public EcmaValue Call(EcmaHeadObject obj, EcmaValue[] arg)
        {
            if (arg.Length == 0)
                return EcmaValue.String("");
            return EcmaValue.String(arg[0].ToString(State));
        }

        public EcmaValue Construct(EcmaValue[] arg)
        {
            return EcmaValue.Object(new StringInstance(this, arg.Length == 0 ? "" : arg[0].ToString(State)));
        }
    }
}

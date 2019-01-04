using Irc.Script.Types.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Function
{
    class FunctionInstance : EcmaHeadObject, ICallable, IConstruct
    {
        private EcmaStatment body;
        private EcmaState State;
        private string[] arg;

        public FunctionInstance(EcmaState state, int length)
        {
            this.Put("length", EcmaValue.Number(length));
            this.Prototype = state.Function;
            this.Put("prototype", EcmaValue.Object(new ObjectInstance(state, new EcmaValue[0])));
            this.Property["prototype"].DontEnum = true;
        }

        public FunctionInstance(EcmaState state, string[] Args, EcmaStatment body) : this(state, Args.Length)
        {
            this.State = state;
            this.body = body;
            this.arg = Args;
        }

        public virtual EcmaValue Call(EcmaHeadObject obj, EcmaValue[] arg)
        {
            EcmaHeadObject argument = new EcmaHeadObject();
            EcmaHeadObject var = new EcmaHeadObject();
            int i = 0;

            for(; i < this.arg.Length; i++)
            {
                if(i < arg.Length)
                {
                    argument.Put(this.arg[i], arg[i]);
                    argument.Put(i.ToString(), arg[i]);
                    var.Put(this.arg[i], arg[i]);
                }
                else
                {
                    var.Put(this.arg[i], EcmaValue.Undefined());
                }
            }

            for (; i < arg.Length; i++)
            {
                argument.Put(i.ToString(), arg[i]);
            }

            argument.Prototype = State.Object;
            argument.Put("length", EcmaValue.Number(arg.Length));
            argument.Property["length"].DontEnum = true;

            var.Put("arguments", EcmaValue.Object(argument));
            var.Put("callee", EcmaValue.Object(this));
            var.Property["callee"].DontEnum = true;

            EcmaHeadObject[] scope = State.GetScope().Clone() as EcmaHeadObject[];
            System.Array.Resize<EcmaHeadObject>(ref scope, scope.Length + 1);
            scope[scope.Length - 1] = var;
            State.PushContext(obj, scope, var);
            
            EcmaComplication com = EcmaEvulator.Evulate(State, this.body);
            State.PopContext();
            if (com.Type == EcmaComplicationType.Return)
                return com.Value;
            return EcmaValue.Undefined();
        }

        public EcmaValue Construct(EcmaValue[] arg)
        {
            EcmaHeadObject obj = this.Get("prototype").ToObject(State);
            EcmaValue value = this.Call(obj, arg);
            if (value.IsObject())
                return value;
            return EcmaValue.Object(obj);
        }
    }
}

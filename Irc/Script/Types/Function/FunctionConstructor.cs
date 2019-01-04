using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Function
{
    class FunctionConstructor : EcmaHeadObject, ICallable, IConstruct
    {
        public EcmaState State;

        public FunctionConstructor(EcmaState state)
        {
            FunctionPrototype prototype = new FunctionPrototype();
            this.Put("prototype", EcmaValue.Object(prototype));
            this.Property["prototype"].DontDelete = true;
            this.Property["prototype"].DontEnum = true;
            this.Property["prototype"].ReadOnly = true;
            this.State = state;
        }

        public EcmaValue Call(EcmaHeadObject self, EcmaValue[] args)
        {
            return Construct(args);
        }

        public EcmaValue Construct(EcmaValue[] args)
        {
            string P = "";
            string Body = "";
            if(args.Length > 0)
            {
                if(args.Length == 1)
                {
                    Body = args[0].ToString(State);
                }
                else
                {
                    P = args[0].ToString(State);
                    for(int i=1;i==args.Length - 1; i++)
                    {
                        P += "," + args[i].ToString(State);
                    }
                    Body = args[args.Length - 1].ToString(State);
                }
            }
            return EcmaValue.Object(new FunctionStringInstance(this, P, Body));
        }
    }
}

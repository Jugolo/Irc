using Irc.Script.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Function
{
    class FunctionStringInstance : EcmaHeadObject, ICallable
    {
        private FunctionConstructor Constructor;
        private string P;
        private string Body;
        private FunctionInstance Cached;

        public FunctionStringInstance(FunctionConstructor constructor, string P, string Body)
        {
            this.Class = "Function";
            this.Prototype = constructor.Get("prototype").ToObject(constructor.State);
            this.Constructor = constructor;
            this.P = P;
            this.Body = Body;
        }

        public override void Put(string P, EcmaValue V)
        {
            if (this.Cached == null)
                this.MakeCache();
            this.Cached.Put(P, V);
        }

        public override EcmaValue Get(string P)
        {
            if (this.Cached == null)
                this.MakeCache();
            return this.Cached.Get(P);
        }

        public EcmaValue Call(EcmaHeadObject self, EcmaValue[] args)
        {
            if(this.Cached == null)
            {
                this.MakeCache();
            }

            return this.Cached.Call(self, args);
        }

        private void MakeCache()
        {
            EcmaState state = this.Constructor.State;
            List<string> args = new List<string>();
            string[] p = this.P.Split(',');
            for (int i = 0; i < p.Length; i++)
            {
                args.Add(p[i].Trim());
            }

            EcmaTokenizer tokenizer = new EcmaTokenizer(new StringReader(this.Body));
            List<EcmaStatment> block = new List<EcmaStatment>();
            while (tokenizer.Current().IsNot(TokenType.EOS))
            {
                block.Add(state.GetProgroam(tokenizer));
            }
            EcmaStatment statment = new EcmaStatment(EcmaStatmentType.Block);
            statment.Statments = block;
            this.Cached = new FunctionInstance(state, args.ToArray(), statment);
        }
    }
}

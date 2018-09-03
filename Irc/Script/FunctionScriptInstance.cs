using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Stack;
using torrent.Script.Values;

namespace torrent.Script
{
    class FunctionScriptInstance : FunctionInstance
    {
        private string name;
        private List<string> args;
        private Statment body;
        private EnergyState state;

        public FunctionScriptInstance(string name, List<string> args, Statment body, EnergyState state)
        {
            this.name = name;
            this.args = args;
            this.body = body;
            this.state = state;
        }

        public string Name()
        {
            return this.name;
        }

        public Value Call(Value[] args)
        {
            VariabelStack stack = new VariabelStack();
            List<Value> a = new List<Value>();

            if (args.Length < this.args.Count)
                throw new ScriptRuntimeException(this.name + " missing arguments");

            int i;
            for (i = 0; i < this.args.Count; i++)
            {
                VariabelDatabaseValue db = new VariabelDatabaseValue();
                db.Context = args[i];
                stack.Put(this.args[i], db);
                a.Add(args[i]);
            }

            for (; i < args.Length; i++)
            {
                //here we put the values there not have a variabels in.
                a.Add(args[i]);
            }

            VariabelDatabaseValue aa = new VariabelDatabaseValue();
            aa.IsLock = true;
            stack.Put("args", aa);

            this.state.pushVariabelStack(stack);
            Complication com = this.state.Evulate(this.body);
            Value var = new NullValue();
            if (com.IsReturn())
                var = com.GetValue();
            this.state.popVariabelStack();
            return var;
        }
    }
}

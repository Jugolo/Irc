using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Stack;
using torrent.Script.Values;

namespace torrent.Script.Refrences
{
    class VariabelRefrence : Refrence
    {
        private VariabelStack stack;
        private string name;

        public VariabelRefrence(VariabelStack stack, string name)
        {
            this.stack = stack;
            this.name = name;
        }

        public Value Get()
        {
            if (this.stack.ContaineVariabel(name))
            {
                return this.stack.Get(name).Context;
            }
            return new NullValue();
        }

        public void Put(Value value)
        {
            VariabelDatabaseValue v;
            if (this.stack.ContaineVariabel(name))
            {
                 v = this.stack.Get(name);
                if (v.IsLock)
                {
                    throw new ScriptRuntimeException("Cant override " + name);
                }
                v.Context = value;
            }
            else
            {
                v = new VariabelDatabaseValue();
                v.Context = value;
                this.stack.Put(name, v);
            }
        }

        public VariabelDatabaseValue GetAttribute()
        {
            return this.stack.GetAttribute(this.name);
        }

        public bool Delete()
        {
            if (this.stack.ContaineVariabel(this.name))
            {
                this.stack.Delete(this.name);
            }

            return true;
        }
    }
}

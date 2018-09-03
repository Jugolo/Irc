using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Values;

namespace torrent.Script.Refrences
{
    class NamedArrayRefrence : Refrence
    {
        private string key;
        private Dictionary<string, Value> obj;

        public NamedArrayRefrence(string key, Dictionary<string, Value> obj)
        {
            this.key = key;
            this.obj = obj;
        }

        public Value Get()
        {
            if (!this.obj.ContainsKey(this.key))
                throw new ScriptRuntimeException("Unknown key in named array: "+this.key);
            return this.obj[this.key];
        }

        public void Put(Value value)
        {
            this.obj[this.key] = value;
        }

        public bool Delete()
        {
            this.obj.Remove(this.key);
            return true;
        }
    }
}

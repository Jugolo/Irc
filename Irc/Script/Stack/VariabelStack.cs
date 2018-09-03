using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Values;

namespace torrent.Script.Stack
{
    public class VariabelStack
    {
        public Dictionary<string, VariabelDatabaseValue> db = new Dictionary<string, VariabelDatabaseValue>();

        public bool ContaineVariabel(string name)
        {
            return this.db.ContainsKey(name);
        }

        public VariabelDatabaseValue Get(string name)
        {
            if (!this.db.ContainsKey(name))
                throw new ScriptRuntimeException("Unknown identify: " + name);
            return this.db[name];
        }

        public void Put(string name, VariabelDatabaseValue value)
        {
            if (this.ContaineVariabel(name))
                throw new ScriptRuntimeException("Value binding already exists: " + name);
            this.db[name] = value;
        }

        public VariabelDatabaseValue GetAttribute(string name)
        {
            if (this.ContaineVariabel(name))
            {
                return this.db[name];
            }

            throw new ScriptRuntimeException("Unknown variabel: " + name);
        }

        public void Delete(string identify)
        {
            if (this.ContaineVariabel(identify))
                this.db.Remove(identify);
        }
    }
}

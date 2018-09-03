using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Values;

namespace torrent.Script.Refrences
{
    class ArrayRefrence : Refrence
    {
        private Value name;
        private List<Value> array;

        public ArrayRefrence(List<Value> array)
        {
            this.array = array;
        }

        public ArrayRefrence(Value index, List<Value> array)
        {
            this.name = index;
            this.array = array;
        }

        public Value Get()
        {
            if (name == null)
                throw new ScriptRuntimeException("Cant get a value from a  array when no index is given");
            return this.array[(int)this.name.ToNumber()];
        }

        public void Put(Value value)
        {
            if (name == null)
                this.array.Add(value);
            else
                this.array[(int)name.ToNumber()] = value;
        }

        public bool Delete()
        {
            if (this.name == null)
                throw new ScriptRuntimeException("Cant delete a array value width no key");
            if (this.array.Count > (int)name.ToNumber())
                this.array.RemoveAt((int)name.ToNumber());
            return true;
        }
    }
}

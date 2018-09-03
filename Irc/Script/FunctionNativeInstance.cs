using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Values;

namespace torrent.Script
{
    class FunctionNativeInstance : FunctionInstance
    {
        public delegate Value NativeCall(Value[] args);

        private NativeCall call;
        private string name;

        public FunctionNativeInstance(String name, NativeCall call)
        {
            this.call = call;
            this.name = name;
        }

        public string Name()
        {
            return this.name;
        }

        public Value Call(Value[] arg)
        {
            return this.call(arg);
        }
    }
}

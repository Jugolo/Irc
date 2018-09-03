using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;

namespace torrent.Script.Values
{
    class FunctionValue : Value
    {
        private FunctionInstance functionInstance;

        public FunctionValue(FunctionInstance functionInstance)
        {
            this.functionInstance = functionInstance;
        }

        public override string Type()
        {
            return "function";
        }

        public override object ToPrimtiv()
        {
            throw new ScriptRuntimeException("Cant get primartiv value of function");
        }

        public override FunctionInstance ToFunction()
        {
            return this.functionInstance;
        }
    }
}

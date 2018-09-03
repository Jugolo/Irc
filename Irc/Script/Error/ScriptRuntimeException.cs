using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Error
{
    class ScriptRuntimeException : ScriptException
    {
        public ScriptRuntimeException(string message)
        {
            this.Message = message;
        }
    }
}

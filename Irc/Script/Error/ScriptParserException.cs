using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Error
{
    class ScriptParserException : ScriptException
    {
        public ScriptParserException(string message)
        {
            this.Message = message;
        }
    }
}

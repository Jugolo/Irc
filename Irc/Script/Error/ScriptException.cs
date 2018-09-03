using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace torrent.Script.Error
{
    class ScriptException : Exception
    {
        public new string Message { get; protected set; }
    }
}

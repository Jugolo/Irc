using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Exceptions
{
    class EcmaRuntimeException : Exception
    {
        public EcmaRuntimeException(string msg) : base(msg)
        {
        }
    }
}

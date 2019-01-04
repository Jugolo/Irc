using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    class EcmaComplication
    {
        public EcmaComplicationType Type { get; private set; }
        public EcmaValue Value { get; private set; }

        public EcmaComplication(EcmaComplicationType type, EcmaValue value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
}

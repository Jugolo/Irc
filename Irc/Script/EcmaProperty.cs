using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    public class EcmaProperty
    {
        public EcmaValue Value { get; set; }
        public bool ReadOnly = false;
        public bool DontDelete = false;
        public bool DontEnum = false;
    }
}

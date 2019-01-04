using Irc.Script.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    class EcmaContext
    {
        public EcmaHeadObject This { get; private set; }
        public EcmaHeadObject[] Scope { get; private set; }
        public EcmaHeadObject Identifys { get; private set; }

        public EcmaContext(EcmaHeadObject t, EcmaHeadObject[] Scope, EcmaHeadObject Identifys)
        {
            this.This = t;
            this.Scope = Scope;
            this.Identifys = Identifys;
        }
    }
}

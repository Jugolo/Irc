using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Object
{
    class ObjectInstance : EcmaHeadObject
    {
        public ObjectInstance(EcmaState state, EcmaValue[] args)
        {
            this.Prototype = state.Object;
            this.Class = "Object";
        }
    }
}

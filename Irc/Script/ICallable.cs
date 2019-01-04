using Irc.Script.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    interface ICallable
    {
        EcmaValue Call(EcmaHeadObject obj, EcmaValue[] args);
    }
}

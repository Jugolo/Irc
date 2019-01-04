using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script.Types.String
{
    class StringInstance : EcmaHeadObject
    {
        public StringInstance(StringConstructor constructor, string str)
        {
            this.Value = EcmaValue.String(str);
            this.Class = "String";
            this.Prototype = constructor.prototype;
            Put("length", EcmaValue.Number(str.Length));
            Property["length"].DontDelete = true;
            Property["length"].DontEnum = true;
            Property["length"].ReadOnly = true;
        }
    }
}

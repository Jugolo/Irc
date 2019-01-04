using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    public class EcmaStatment
    {
        public EcmaStatmentType Type { get; private set; }
        public string Name { get; set; }
        public EcmaStatment Statment { get; set; }
        public List<EcmaStatment> Statments { get; set; }
        internal ExpresionData Expresion { get; set; }
        public EcmaStatment Else { get; internal set; }
        internal ExpresionData Second { get; set; }
        internal ExpresionData Tree { get; set; }
        public string[] Args { get; internal set; }
        internal Dictionary<string, ExpresionData> VarList { get; set; }

        public EcmaStatment(EcmaStatmentType type)
        {
            this.Type = type;
        }
    }
}

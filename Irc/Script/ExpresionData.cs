using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    class ExpresionData
    {
        public ExpresionType Type { get; private set; }
        public List<ExpresionData> Multi { get; internal set; }
        public string Name { get; internal set; }
        public ExpresionData Left { get; internal set; }
        public ExpresionData Right { get; internal set; }
        public string Sign { get; internal set; }
        public ExpresionData Test { get; internal set; }
        public ExpresionData[] Arg { get; internal set; }

        public ExpresionData(ExpresionType type)
        {
            this.Type = type;
        }
    }
}

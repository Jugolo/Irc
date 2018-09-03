using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc
{
    class TextSize
    {
        private int LineCount = 0;
        public int StartLine { get; private set; }

        public TextSize(int startline)
        {
            this.StartLine = startline;
        }

        public int GetY()
        {
            return (this.LineCount - this.StartLine) * 12;
        }

        public void NextLine()
        {
            this.LineCount++;
        }

        public bool ShouldShow()
        {
            if (this.LineCount < this.StartLine)
                return false;
            return true;
        }
    }
}

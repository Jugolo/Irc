using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Token
{
    public class EcmaReader
    {
        private TextReader Reader;
        public int Line { get; private set; }

        public EcmaReader(TextReader reader)
        {
            this.Reader = reader;
            this.Line = 1;
        }

        public int Read()
        {
            int c = this.Reader.Read();
            if (c == '\n')
                this.Line++;
            return c;
        }

        public int Peek()
        {
            return this.Reader.Peek();
        }
    }
}

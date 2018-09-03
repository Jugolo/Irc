using System.IO;

namespace torrent.Script
{
    public class InputTextReader : InputReader
    {
        private TextReader reader;

        public int Line { get; private set; }
        public int Row { get; private set; }

        public InputTextReader(TextReader reader)
        {
            this.Line = 1;
            this.Row = 0;
            this.reader = reader;
        }

        public bool IsEnd()
        {
            return this.reader.Peek() == -1;
        }

        public int Read()
        {
            int c = this.reader.Read();
            if(c == '\n')
            {
                Line++;
                Row = 0;
            }
            else
            {
                Row++;
            }
            return c;
        }

        public int Peek()
        {
            return this.reader.Peek();
        }
    }
}
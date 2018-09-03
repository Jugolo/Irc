using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Libary;
using torrent.Script.Refrences;
using torrent.Script.Stack;
using torrent.Script.Token;
using torrent.Script.Values;

namespace torrent.Script
{
    class Energy
    {
        public EnergyState state { get; private set; }
        private VariabelDatabase variabel = new VariabelDatabase();
        private Complication last;

        public Energy()
        {
            this.state = new EnergyState();
            this.SetLibary();
        }

        public void Parse(string text)
        {
            Parse(new StringReader(text));
        }

        public void Parse(TextReader reader)
        {
            Parse(new InputTextReader(reader));
        }

        public void Parse(InputReader reader)
        {
            Tokenizer token = new Tokenizer(reader);
            while (!token.Current().Is("EOS"))
            {
                this.last = this.state.Evulate(this.state.ParseNext(token));
            }
        }

        public Value GetVariabel(string name)
        {
            return this.state.GetVariabel(name).Get();
        }

        public bool HasValue(string key)
        {
            for(int i = 0; i < this.state.VarDB.Size(); i++)
            {
                if (this.state.VarDB.Get(i).ContaineVariabel(key))
                    return true;
            }
            return false;
        }

        public VariabelDatabaseValue PutValue(string name, Value value)
        {
            VariabelRefrence r = this.state.GetVariabel(name);
            r.Put(value);
            return r.GetAttribute();
        }

        public Complication GetLast()
        {
            return this.last;
        }

        public void SetLibary()
        {
            ScriptLibary.Init(this);
        }
    }
}

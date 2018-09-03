using System.Collections.Generic;

namespace torrent.Script
{
    public class Statment
    {
        private Expresion expresion;
        private Expresion two;
        private Expresion thried;
        private List<Statment> block;
        private StatmentType type;
        private string name;
        private string value;
        private List<string> args;
        private Statment body;
        private Statment extraStatment;

        public Statment(StatmentType type, Expresion expresion, string name, string value, Statment body)
        {
            this.type = type;
            this.expresion = expresion;
            this.name = name;
            this.value = value;
            this.body = body;
        }

        public Statment(StatmentType type, Expresion first, Expresion second, Expresion thrid, Statment body)
        {
            this.type = type;
            this.expresion = first;
            this.two = second;
            this.thried = thrid;
            this.body = body;

        }

        public  Statment(StatmentType type, string name, Expresion exp)
        {
            this.type = type;
            this.name = name;
            this.expresion = exp;
        }

        public Statment(StatmentType type, Expresion exp)
        {
            this.type = type;
            this.expresion = exp;
        }

        public Statment(Expresion expresion)
        {
            this.expresion = expresion;
            this.type = StatmentType.Expresion;
        }

        public Statment(List<Statment> statment)
        {
            this.block = statment;
            this.type = StatmentType.Block;
        }

        public Statment(StatmentType type, Expresion controle, Statment body)
        {
            this.type = type;
            this.body = body;
            this.expresion = controle;
        }

        public Statment(string name, List<string> args, Statment body)
        {
            this.name = name;
            this.args = args;
            this.body = body;
            this.type = StatmentType.Function;
        }

        public Statment(StatmentType type, Expresion controle, Statment block, Statment secound)
        {
            this.type = type;
            this.expresion = controle;
            this.body = block;
            this.extraStatment = secound;
        }

        public StatmentType Type()
        {
            return this.type;
        }

        public string Name()
        {
            return this.name;
        }

        public string Value()
        {
            return this.value;
        }

        public List<string> Args()
        {
            return this.args;
        }

        public List<Statment> Block()
        {
            return this.block;
        }

        public Statment Body()
        {
            return this.body;
        }

        public Expresion Expresion()
        {
            return this.expresion;
        }

        public Statment SecoundStatment()
        {
            return this.extraStatment;
        }

        public Expresion SecondExpresion()
        {
            return this.two;
        }

        public Expresion ThirdExpresion()
        {
            return this.thried;
        }
    }
}
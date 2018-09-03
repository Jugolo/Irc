using System;
using System.Collections.Generic;
using torrent.Script.Error;
using torrent.Script.Token;

namespace torrent.Script
{
    public class Expresion
    {
        public ExpresionType Type { get; private set; }
        public string Name { get; private set; }
        public List<Expresion> Args { get; private set; }
        public Expresion Left { get; private set; }
        public Expresion Right { get; private set; }
        public Dictionary<Expresion, Expresion> NamedArray { get; private set; }
        public List<Expresion> Array { get; private set; }
        public List<string> FuncArgs { get; private set; }
        public Statment Body { get; private set; }

        public static Expresion Empty()
        {
            Expresion exp = new Expresion();
            exp.Type = ExpresionType.Empty;
            return exp;
        }

        internal static Expresion Parse(Tokenizer token, EnergyState state)
        {
            return Assign(token, state);
        }

        private static Expresion Assign(Tokenizer token, EnergyState state)
        {
            Expresion exp = Bind(token, state);

            if(token.Current().Is("punctor", "="))
            {
                token.Next();
                Expresion e = new Expresion();
                e.Type = ExpresionType.Assign;
                e.Left = exp;
                e.Right = Assign(token, state);
                return e;
            }

            return exp;
        }

        private static Expresion Bind(Tokenizer token, EnergyState state)
        {
            Expresion expresion = Equeals(token, state);
            if (token.Current().Is("punctor", "&&"))
            {
                token.Next();
                Expresion bind = new Expresion();
                bind.Left = expresion;
                bind.Type = ExpresionType.AND;
                bind.Right = Bind(token, state);
                return bind;
            }
            return expresion;
        }

        private static Expresion Equeals(Tokenizer token, EnergyState state)
        {
            Expresion expresion = Relation(token, state);
            if(token.Current().Is("punctor", "==") || token.Current().Is("punctor", "!="))
            {
                string sign = token.Current().Context();
                Expresion exp = new Expresion();
                exp.Left = expresion;
                exp.Type = token.Current().Is("punctor", "==") ? ExpresionType.Equel : ExpresionType.NotEqual;
                token.Next();
                exp.Right = Prefix(token, state);
                return exp;
            }
            return expresion;
        }

        private static Expresion Relation(Tokenizer token, EnergyState state)
        {
            Expresion exp = Math(token, state);
            if(token.Current().Is("punctor", "<") || token.Current().Is("punctor", ">") || token.Current().Is("punctor", "<=") || token.Current().Is("punctor", ">="))
            {
                string sign = token.Current().Context();
                token.Next();
                Expresion s = new Expresion();
                s.Left = exp;
                s.Type = sign == "<" ? ExpresionType.LEFT_ARROW :
                    sign == ">" ? ExpresionType.RIGHT_ARROW :
                    sign == ">=" ? ExpresionType.CREATOR : ExpresionType.LOWER;
                s.Right = Relation(token, state);
                return s;
            }
            return exp;
        }

        private static Expresion Math(Tokenizer token, EnergyState state)
        {
            Expresion expresion = Self(token, state);
            if(token.Current().Is("punctor", "+") || token.Current().Is("punctor", "-"))
            {
                Expresion e = new Expresion();
                e.Left = expresion;
                e.Type = token.Current().Context() == "+" ? ExpresionType.Plus : ExpresionType.Minus;
                token.Next();
                e.Right = Math(token, state);
                return e;
            }
            return expresion;
        }

        private static Expresion Self(Tokenizer token, EnergyState state)
        {
            Expresion exp = Prefix(token, state);
            
            if(token.Current().Is("punctor", "++") || token.Current().Is("punctor", "--"))
            {
                Expresion e = new Expresion();
                e.Left = exp;
                e.Type = token.Current().Context() == "++" ? ExpresionType.SelfPlus : ExpresionType.SelfMinus;
                token.Next();
                e.Right = GetNumber("1");
                return e;
            }

            return exp;
        }

        private static Expresion Prefix(Tokenizer token, EnergyState state)
        {
            if(token.Current().Is("punctor", "!"))
            {
                token.Next();
                Expresion e = new Expresion();
                e.Left = Primary(token, state);
                e.Type = ExpresionType.Not;
                return e;
            }

            if(token.Current().Is("punctor", "-"))
            {
                token.Next();
                Expresion e = new Expresion();
                e.Left = Primary(token, state);
                e.Type = ExpresionType.Negetiv;
                return e;
            }

            if(token.Current().Is("punctor", "+"))
            {
                token.Next();
                Expresion e = new Expresion();
                e.Left = Primary(token, state);
                e.Type = ExpresionType.Posetiv;
                return e;
            }

            return Primary(token, state);
        }

        private static Expresion Primary(Tokenizer token, EnergyState state)
        {
            TokenBuffer buffer = token.Current();
            token.Next();
            Expresion exp = new Expresion();

            if (buffer.Is("identify"))
            {
                exp.Type = ExpresionType.Identify;
                exp.Name = buffer.Context();
                return HandleAfterVariabel(exp, token, state);
            }

            if (buffer.Is("string"))
            {
                exp.Type = ExpresionType.String;
                exp.Name = buffer.Context();
                return exp;
            }

            if (buffer.Is("bool"))
            {
                exp.Type = ExpresionType.Bool;
                exp.Name = buffer.Context();
                return exp;
            }

            if (buffer.Is("number"))
            {
                exp.Type = ExpresionType.Number;
                exp.Name = buffer.Context();
                return exp;
            }

            if (buffer.Is("punctor", "{"))
            {
                return GetNamedArray(token, state);
            }

            if(buffer.Is("punctor", "["))
            {
                return GetArray(token, state);
            }

            if(buffer.Is("keyword", "function"))
            {
                return GetFunction(token, state);
            }

            throw new ScriptParserException("Unexpected token detected: " + buffer.Context() + "(" + buffer.Type() + ")");
        }

        private static Expresion GetFunction(Tokenizer token, EnergyState state)
        {
            token.Current().Expect("punctor", "(");
            List<string> args = new List<string>();
            if(!token.Next().Is("punctor", ")"))
            {
                args.Add(token.Current().Expect("identify").Context());
                while(token.Next().Is("punctor", ","))
                {
                    token.Next();
                    args.Add(token.Current().Context());
                }
            }

            token.Current().Expect("punctor", ")");
            Statment body = state.GetBlock(token);
            Expresion expresion = new Expresion();
            expresion.Type = ExpresionType.Function;
            expresion.FuncArgs = args;
            expresion.Body = body;
            return expresion;
        }

        private static Expresion GetNumber(string number)
        {
            Expresion exp = new Expresion();
            exp.Type = ExpresionType.Number;
            exp.Name = number;
            return exp;
        }

        private static Expresion HandleAfterVariabel(Expresion exp, Tokenizer token, EnergyState state)
        {
            if(token.Current().Is("punctor", "("))
            {
                //function call or method(When this is done ;)
                Expresion call = new Expresion();
                call.Type = ExpresionType.Call;
                call.Left = exp;
                List<Expresion> args = new List<Expresion>();
                if(!token.Next().Is("punctor", ")"))
                {
                    args.Add(Parse(token, state));
                    while(token.Current().Is("punctor", ","))
                    {
                        token.Next();
                        args.Add(Parse(token, state));
                    }
                }
                call.Args = args;
                token.Current().Expect("punctor", ")");
                token.Next();
                return HandleAfterVariabel(call, token, state);
            }else if(token.Current().Is("punctor", "["))
            {
                Expresion get = new Expresion();
                get.Type = ExpresionType.ArrayGet;
                token.Next();
                get.Left = exp;
                if(!token.Current().Is("punctor", "]"))
                    get.Right = Parse(token, state);
                token.Current().Expect("punctor", "]");
                token.Next();
                return HandleAfterVariabel(get, token, state);
            }
            return exp;
        }


        private static Expresion GetArray(Tokenizer token, EnergyState state)
        {
            List<Expresion> array = new List<Expresion>();
            if(!token.Current().Is("punctor", "]"))
            {
                array.Add(Parse(token, state));
                while(token.Current().Is("punctor", ","))
                {
                    token.Next();
                    array.Add(Parse(token, state));
                }
            }
            token.Current().Expect("punctor", "]");
            token.Next();
            Expresion expresion = new Expresion();
            expresion.Type = ExpresionType.Array;
            expresion.Array = array;
            return expresion;
        }

        private static Expresion GetNamedArray(Tokenizer token, EnergyState state)
        {
            Dictionary<Expresion, Expresion> values = new Dictionary<Expresion, Expresion>();
            if (!token.Current().Is("punctor", "}"))
            {
                NextNamedArrayItem(values, token, state);
                while(token.Current().Is("punctor", ",")){
                    token.Next();
                    NextNamedArrayItem(values, token, state);
                } 
            }
            token.Current().Expect("punctor", "}");
            token.Next();
            Expresion expresion = new Expresion();
            expresion.NamedArray = values;
            expresion.Type = ExpresionType.NamedArray;
            return expresion;
        }

        private static void NextNamedArrayItem(Dictionary<Expresion, Expresion> container, Tokenizer token, EnergyState state)
        {
            Expresion e = Parse(token, state);
            token.Current().Expect("punctor", ":");
            token.Next();
            container.Add(e, Parse(token, state));
        }
    }
}
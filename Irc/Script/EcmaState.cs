using Irc.Script.Exceptions;
using Irc.Script.Token;
using Irc.Script.Types;
using Irc.Script.Types.Array;
using Irc.Script.Types.Function;
using Irc.Script.Types.Object;
using Irc.Script.Types.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script
{
    public class EcmaState
    {
        private string[] AssignOperator = new string[]
        {
            "=",
            "*=",
            "/=",
            "%=",
            "+=",
            "-=",
            "<<=",
            ">>=",
            ">>>=",
            "&=",
            "^=",
            "|="
        };

        private string[] RelationalOperator = new string[]
        {
            "<",
            ">",
            "<=",
            ">="
        };

        private string[] ShiftOperator = new string[]
        {
            "<<",
            ">>",
            ">>>"
        };

        private List<EcmaContext> Identifys = new List<EcmaContext>();
        
        public ObjectPrototype Object { get; set; }
        public StringConstructor String { get; set; }
        public FunctionPrototype Function { get; set; }
        public ArrayPrototype Array { get; set; }
        public GlobalObject GlobalObject { get; private set; }
        
        public bool InitedStandartLibary { get; private set; }

        public void BuildStandartLibary()
        {
            if (!this.InitedStandartLibary)
            {
                this.GlobalObject = new GlobalObject(this);
                this.PushContext(this.GlobalObject, this.GlobalObject, this.GlobalObject);
                this.InitedStandartLibary = true;
            }

        }

        public void PushContext(EcmaHeadObject This, EcmaHeadObject Scope, EcmaHeadObject Identifys)
        {
            this.PushContext(This, new EcmaHeadObject[] { Scope }, Identifys);
        }

        public void PushContext(EcmaHeadObject This, EcmaHeadObject[] Scope, EcmaHeadObject identifys)
        {
            this.Identifys.Add(new EcmaContext(This, Scope, identifys));
        }

        public void PopContext()
        {
            this.Identifys.RemoveAt(this.Identifys.Count - 1);
        }

        public EcmaHeadObject[] GetScope()
        {
            return this.Identifys[this.Identifys.Count - 1].Scope;
        }

        public Reference GetIdentify(string name)
        {
            if (this.Identifys.Count == 0)
                return new Reference(name);
            //Get the last scope array
            EcmaHeadObject[] obj = this.Identifys[this.Identifys.Count - 1].Scope;
            for(int i = obj.Length - 1; i >= 0; i--)
            {
                if (obj[i].HasProperty(name))
                {
                    return new Reference(name, obj[i]);
                }
            }

            return new Reference(name);
        }

        public EcmaHeadObject GetThis()
        {
            return this.Identifys[this.Identifys.Count - 1].This;
        }

        public EcmaStatment GetProgroam(EcmaTokenizer token)
        {
            if(token.Current().Is(TokenType.Keyword, "function"))
            {
                return GetFunctionDec(token);
            }
            return GetStatment(token);
        }

        private EcmaStatment GetBlock(EcmaTokenizer token)
        {
            List<EcmaStatment> elem = new List<EcmaStatment>();
            if(token.Next().IsNot(TokenType.Punctor, "}")){
                elem.Add(GetStatment(token));
                while(token.Current().IsNot(TokenType.Punctor, "}"))
                {
                    elem.Add(GetStatment(token));
                }
            }

            token.Current().Excepect(TokenType.Punctor, "}");
            token.Next();
            EcmaStatment statment = new EcmaStatment(EcmaStatmentType.Block);
            statment.Statments = elem;
            return statment;
        }

        private EcmaStatment GetFunctionDec(EcmaTokenizer token)
        {
            token.Next().Excepect(TokenType.Identify);
            EcmaStatment statment = new EcmaStatment(EcmaStatmentType.FunctionDec);
            statment.Name = token.Current().Context;
            token.Next().Excepect(TokenType.Punctor, "(");
            List<String> args = new List<String>();
            if(token.Next().IsNot(TokenType.Punctor, ")"))
            {
                token.Current().Excepect(TokenType.Identify);
                args.Add(token.Current().Context);
                while(token.Next().Is(TokenType.Punctor, ","))
                {
                    token.Next().Excepect(TokenType.Identify);
                    args.Add(token.Current().Context);
                }
            }

            token.Current().Excepect(TokenType.Punctor, ")");
            token.Next().Excepect(TokenType.Punctor, "{");
            statment.Args = args.ToArray();
            statment.Statment = GetBlock(token);


            return statment;
        }

        private EcmaStatment GetStatment(EcmaTokenizer token)
        {
            if (token.Current().Is(TokenType.Keyword))
            {
                switch (token.Current().Context)
                {
                    case "if":
                        return GetIf(token);
                    case "return":
                        return GetReturn(token);
                    case "for":
                        return GetFor(token);
                    case "while":
                        return GetWhile(token);
                    case "var":
                        return GetVar(token);
                    case "continue":
                        return GetContinue(token);
                }
            }

            if(token.Current().Is(TokenType.Punctor, "{"))
            {
                return GetBlock(token);
            }

            return StatmentExpresion(token);
        }

        private EcmaStatment GetContinue(EcmaTokenizer token)
        {
            token.Next().Excepect(TokenType.Punctor, ";");
            token.Next();
            return new EcmaStatment(EcmaStatmentType.Continue);
        }

        private EcmaStatment GetVar(EcmaTokenizer token)
        {
            Dictionary<string, ExpresionData> list = new Dictionary<string, ExpresionData>();
            token.Next().Excepect(TokenType.Identify);
            string identify = token.Current().Context;
            if(token.Next().Is(TokenType.Punctor, "=")){
                token.Next();
                list.Add(identify, Expresion(token));
            }
            else
            {
                list.Add(identify, null);
            }

            while(token.Current().Is(TokenType.Punctor, ","))
            {
                token.Next().Excepect(TokenType.Identify);
                identify = token.Current().Context;
                if (token.Next().Is(TokenType.Punctor, "="))
                {
                    token.Next();
                    list.Add(identify, Expresion(token));
                }
                else
                {
                    list.Add(identify, null);
                }
            }

            token.Current().Excepect(TokenType.Punctor, ";");
            token.Next();

            EcmaStatment statment = new EcmaStatment(EcmaStatmentType.Var);
            statment.VarList = list;
            return statment;
        }

        private EcmaStatment GetWhile(EcmaTokenizer token)
        {
            token.Next().Excepect(TokenType.Punctor, "(");
            token.Next();
            EcmaStatment statment = new EcmaStatment(EcmaStatmentType.While);
            statment.Expresion = Expresion(token);
            token.Current().Excepect(TokenType.Punctor, ")");
            token.Next();
            statment.Statment = GetStatment(token);
            return statment;
        }

        private ExpresionData VariableDeclarationList(EcmaTokenizer token)
        {
            List<ExpresionData> list = new List<ExpresionData>();
            ExpresionData exp;
            token.Current().Excepect(TokenType.Identify);
            exp = new ExpresionData(ExpresionType.Assign);
            ExpresionData identify = new ExpresionData(ExpresionType.Identify);
            identify.Name = token.Current().Context;
            exp.Left = identify;
            exp.Sign = "=";
            if(token.Next().Is(TokenType.Punctor, "="))
            {
                token.Next();
                exp.Right = AssignmentExpression(token);
            }
            else
            {
                exp.Right = new ExpresionData(ExpresionType.Null);
            }
            list.Add(exp);
            while(token.Current().Is(TokenType.Punctor, ","))
            {
                exp = new ExpresionData(ExpresionType.Assign);
                token.Next().Excepect(TokenType.Identify);
                identify = new ExpresionData(ExpresionType.Identify);
                identify.Name = token.Current().Context;
                exp.Left = identify;
                exp.Sign = "=";
                if(token.Next().Is(TokenType.Punctor, "="))
                {
                    token.Next();
                    exp.Right = AssignmentExpression(token);
                }
                else
                {
                    exp.Right = new ExpresionData(ExpresionType.Null);
                }
                list.Add(exp);
            }

            ExpresionData varList = new ExpresionData(ExpresionType.VarList);
            varList.Multi = list;
            return varList;
        }

        private EcmaStatment GetFor(EcmaTokenizer token)
        {
            token.Next().Excepect(TokenType.Punctor, "(");
            ExpresionData first = new ExpresionData(ExpresionType.Null);
            if(token.Next().IsNot(TokenType.Punctor, ";"))
            {
                if(token.Current().Is(TokenType.Keyword, "var"))
                {
                    token.Next();
                    first = VariableDeclarationList(token);
                }
                else
                {
                    first = Expresion(token);
                }
            }
            EcmaStatment current = null;

            if(token.Current().Is(TokenType.Keyword, "in"))
            {
                token.Next();
                current = new EcmaStatment(EcmaStatmentType.ForIn);
                current.Expresion = first;
                current.Second = Expresion(token);
            }
            else
            {
                token.Current().Excepect(TokenType.Punctor, ";");
                token.Next();
                current = new EcmaStatment(EcmaStatmentType.For);
                current.Expresion = first;
                if (token.Current().IsNot(TokenType.Punctor, ";"))
                {
                    current.Second = Expresion(token);
                    token.Current().Excepect(TokenType.Punctor, ";");
                }
                else
                    current.Second = new ExpresionData(ExpresionType.Null);

                if (token.Next().IsNot(TokenType.Punctor, ")"))
                {
                    current.Tree = Expresion(token);
                }
                else
                    current.Tree = new ExpresionData(ExpresionType.Null);
            }

            token.Current().Excepect(TokenType.Punctor, ")");
            token.Next();
            current.Statment = GetStatment(token);
            return current;
        }

        private EcmaStatment GetReturn(EcmaTokenizer token)
        {
            EcmaStatment statment = new EcmaStatment(EcmaStatmentType.Return);
            if(token.Current().LineStart == token.Next().LineStart)
            {
                if (token.Current().IsNot(TokenType.Punctor, ";") && token.Current().IsNot(TokenType.EOS))
                {
                    statment.Expresion = Expresion(token);
                    if (token.Lines[token.Lines.Count - 2] == token.Current().LineStart)
                    {
                        if (token.Current().IsNot(TokenType.Punctor, ";") && token.Current().IsNot(TokenType.EOS))
                            token.Current().Excepect(TokenType.Punctor, ";");
                        token.Next();
                    }
                }
                else
                {
                    token.Next();
                }
            }
            return statment;
        }

        private EcmaStatment GetIf(EcmaTokenizer token)
        {
            token.Next().Excepect(TokenType.Punctor, "(");
            token.Next();
            EcmaStatment statment = new EcmaStatment(EcmaStatmentType.If);
            statment.Expresion = Expresion(token);
            token.Current().Excepect(TokenType.Punctor, ")");
            token.Next();
            statment.Statment = GetStatment(token);
            if(token.Current().Is(TokenType.Keyword, "else"))
            {
                token.Next();
                statment.Else = GetStatment(token);
            }
            return statment;
        }

        private EcmaStatment StatmentExpresion(EcmaTokenizer token)
        {
            EcmaStatment statment = new EcmaStatment(EcmaStatmentType.Expresion);
            statment.Expresion = Expresion(token);
            if (token.Current().IsNot(TokenType.Punctor, ";") && token.Current().IsNot(TokenType.EOS))
            {
                if (token.Lines[token.Lines.Count - 2] == token.Current().LineStart)
                    token.Current().Excepect(TokenType.Punctor, ";");
            }
            else
                token.Next();
            return statment;
        }

        private ExpresionData Expresion(EcmaTokenizer token)
        {
            ExpresionData expresion = AssignmentExpression(token);
            if(token.Current().Is(TokenType.Punctor, ","))
            {
                ExpresionData buffer = new ExpresionData(ExpresionType.MultiExpresion);
                buffer.Multi = new List<ExpresionData>();
                buffer.Multi.Add(expresion);
                while(token.Current().Is(TokenType.Punctor, ","))
                {
                    token.Next();
                    buffer.Multi.Add(AssignmentExpression(token));
                }
                return buffer;
            }

            return expresion;
        }

        private ExpresionData AssignmentExpression(EcmaTokenizer token)
        {
            ExpresionData exp = ConditionalExpression(token);
            
            if(token.Current().Is(TokenType.Punctor, AssignOperator))
            {
                ExpresionData buf = new ExpresionData(ExpresionType.Assign);
                buf.Left = exp;
                buf.Sign = token.Current().Context;
                token.Next();
                buf.Right = AssignmentExpression(token);
                return buf;
            }

            return exp;
        }

        private ExpresionData ConditionalExpression(EcmaTokenizer token)
        {
            ExpresionData exp = LogicalORExpression(token);
            if(token.Current().Is(TokenType.Punctor, "?"))
            {
                token.Next();
                ExpresionData buf = new ExpresionData(ExpresionType.Conditional);
                buf.Test = exp;
                buf.Left = AssignmentExpression(token);
                token.Current().Excepect(TokenType.Punctor, ":");
                token.Next();
                buf.Right = AssignmentExpression(token);
                return buf;
            }
            return exp;
        }

        private ExpresionData LogicalORExpression(EcmaTokenizer token)
        {
            ExpresionData exp = LogicalANDExpression(token);
            if(token.Current().Is(TokenType.Punctor, "||"))
            {
                token.Next();
                ExpresionData buf = new ExpresionData(ExpresionType.Or);
                buf.Left = exp;
                buf.Right = LogicalORExpression(token);
                return buf;
            }
            return exp;
        }

        private ExpresionData LogicalANDExpression(EcmaTokenizer token)
        {
            ExpresionData exp = BitwiseORExpression(token);
            if(token.Current().Is(TokenType.Punctor, "&&"))
            {
                token.Next();
                ExpresionData buf = new ExpresionData(ExpresionType.AND);
                buf.Left = exp;
                buf.Right = LogicalANDExpression(token);
                return buf;
            }
            return exp;
        }

        private ExpresionData BitwiseORExpression(EcmaTokenizer token)
        {
            ExpresionData exp = BitwiseXORExpression(token);
            if(token.Current().Is(TokenType.Punctor, "|"))
            {
                token.Next();
                ExpresionData buf = new ExpresionData(ExpresionType.BOR);
                buf.Left = exp;
                buf.Right = BitwiseORExpression(token);
                return buf;
            }

            return exp;
        }

        private ExpresionData BitwiseXORExpression(EcmaTokenizer token)
        {
            ExpresionData exp = BitwiseANDExpression(token);
            if(token.Current().Is(TokenType.Punctor, "^"))
            {
                token.Next();
                ExpresionData buf = new ExpresionData(ExpresionType.XOR);
                buf.Left = exp;
                buf.Right = BitwiseXORExpression(token);
                return buf;
            }

            return exp;
        }

        private ExpresionData BitwiseANDExpression(EcmaTokenizer token)
        {
            ExpresionData exp = EqualityExpression(token);
            if(token.Current().Is(TokenType.Punctor, "&"))
            {
                token.Next();
                ExpresionData buf = new ExpresionData(ExpresionType.BAND);
                buf.Left = exp;
                buf.Right = BitwiseANDExpression(token);
                return buf;
            }
            return exp;
        }

        private ExpresionData EqualityExpression(EcmaTokenizer token)
        {
            ExpresionData exp = RelationalExpression(token);
            if(token.Current().Is(TokenType.Punctor, new string[] { "==", "!=" }))
            {
                ExpresionData buf = new ExpresionData(ExpresionType.Equlity);
                buf.Left = exp;
                buf.Sign = token.Current().Context;
                token.Next();
                buf.Right = RelationalExpression(token);
                return buf;
            }
            return exp;
        }

        private ExpresionData RelationalExpression(EcmaTokenizer token)
        {
            ExpresionData exp = ShiftExpression(token);
            if(token.Current().Is(TokenType.Punctor, RelationalOperator))
            {
                ExpresionData buf = new ExpresionData(ExpresionType.Relational);
                buf.Left = exp;
                buf.Sign = token.Current().Context;
                token.Next();
                buf.Right = ShiftExpression(token);
                return buf;
            }
            return exp;
        }

        private ExpresionData ShiftExpression(EcmaTokenizer token)
        {
            ExpresionData exp = AdditiveExpression(token);
            if(token.Current().Is(TokenType.Punctor, ShiftOperator))
            {
                ExpresionData buf = new ExpresionData(ExpresionType.Shift);
                buf.Left = exp;
                buf.Sign = token.Current().Context;
                token.Next();
                buf.Right = ShiftExpression(token);
                return buf;
            }

            return exp;
        }

        private ExpresionData AdditiveExpression(EcmaTokenizer token)
        {
            ExpresionData exp = MultiplicativeExpression(token);
            if(token.Current().Is(TokenType.Punctor, new string[] { "-", "+" }))
            {
                ExpresionData buf = new ExpresionData(ExpresionType.Additive);
                buf.Left = exp;
                buf.Sign = token.Current().Context;
                token.Next();
                buf.Right = AdditiveExpression(token);
                return buf;
            }
            return exp;
        }

        private ExpresionData MultiplicativeExpression(EcmaTokenizer token) {
            ExpresionData exp = UnaryExpression(token);
            if(token.Current().Is(TokenType.Punctor, new string[] { "*", "/", "%" }))
            {
                ExpresionData buf = new ExpresionData(ExpresionType.Multiplicative);
                buf.Sign = token.Current().Context;
                token.Next();
                buf.Left = exp;
                buf.Right = MultiplicativeExpression(token);
                return buf;
            }
            return exp;
        }

        private ExpresionData UnaryExpression(EcmaTokenizer token)
        {
            if(token.Current().Is(TokenType.Keyword, new string[] { "delete", "void", "typeof" }) || token.Current().Is(TokenType.Punctor, new string[] { "++", "--", "+", "-", "~", "!"}))
            {
                ExpresionData buf = new ExpresionData(ExpresionType.Unary);
                buf.Sign = token.Current().Context;
                token.Next();
                buf.Left = UnaryExpression(token);
                return buf;
            }

            return PostfixExpression(token);
        }

        private ExpresionData PostfixExpression(EcmaTokenizer token)
        {
            ExpresionData exp = LeftHandSideExpression(token);
            if(token.Current().Is(TokenType.Punctor, new string[] { "--", "++" }))
            {
                ExpresionData buf = new ExpresionData(ExpresionType.Unary);
                buf.Sign = token.Current().Context;
                buf.Left = exp;
                token.Next();
                return buf;
            }
            return exp;
        }

        private ExpresionData LeftHandSideExpression(EcmaTokenizer token)
        {
            if (token.Current().Is(TokenType.Keyword, "new"))
                return NewExpression(token);

            ExpresionData exp = PrimaryExpresion(token);
            ExpresionData buf = null;
            while(token.Current().Is(TokenType.Punctor, new string[] { ".", "(", "[" }))
            {
                switch (token.Current().Context)
                {
                    case ".":
                        token.Next().Excepect(TokenType.Identify);
                        buf = new ExpresionData(ExpresionType.ObjGet);
                        buf.Left = exp;
                        buf.Sign = token.Current().Context;
                        token.Next();
                        exp = buf;
                        break;
                    case "[":
                        token.Next();
                        buf = new ExpresionData(ExpresionType.ItemGet);
                        buf.Left = exp;
                        buf.Right = Expresion(token);
                        token.Current().Excepect(TokenType.Punctor, "]");
                        token.Next();
                        break;
                    case "(":
                        buf = new ExpresionData(ExpresionType.Call);
                        buf.Left = exp;
                        buf.Arg = Arguments(token);
                        break;
                }
                exp = buf;
            }
            return exp;
        }

        private ExpresionData NewExpression(EcmaTokenizer token)
        {
            ExpresionData exp = new ExpresionData(ExpresionType.New);
            token.Next();
            exp.Left = PrimaryExpresion(token);
            token.Current().Excepect(TokenType.Punctor, "(");
            exp.Arg = Arguments(token);
            return exp;
        }

        private ExpresionData PrimaryExpresion(EcmaTokenizer token)
        {
            TokenBuffer buf = token.Current();
            token.Next();

            if(buf.Is(TokenType.Keyword, "this"))
            {
                return new ExpresionData(ExpresionType.This);
            }

            if (buf.Is(TokenType.Number))
            {
                ExpresionData n = new ExpresionData(ExpresionType.Number);
                n.Sign = buf.Context;
                return n;
            }

            if (buf.Is(TokenType.Null))
            {
                return new ExpresionData(ExpresionType.Null);
            }

            if (buf.Is(TokenType.Bool))
            {
                ExpresionData b = new ExpresionData(ExpresionType.Bool);
                b.Sign = buf.Context;
                return b;
            }

            if (buf.Is(TokenType.Identify))
            {
                ExpresionData identify = new ExpresionData(ExpresionType.Identify);
                identify.Name = buf.Context;
                return identify;
            }

            if (buf.Is(TokenType.String))
            {
                ExpresionData str = new ExpresionData(ExpresionType.String);
                str.Sign = buf.Context;
                return str;
            }

            if(buf.Is(TokenType.Punctor, "("))
            {
                ExpresionData p = Expresion(token);
                token.Current().Excepect(TokenType.Punctor, ")");
                token.Next();
                return p;
            }

            throw new EcmaRuntimeException("Unknown token detected "+buf.Context+"("+buf.Type.ToString()+") on line "+buf.LineStart);
        }

        private ExpresionData[] Arguments(EcmaTokenizer token)
        {
            List<ExpresionData> args = new List<ExpresionData>();
            if(token.Next().IsNot(TokenType.Punctor, ")"))
            {
                args.Add(AssignmentExpression(token));
                while(token.Current().Is(TokenType.Punctor, ","))
                {
                    token.Next();
                    args.Add(AssignmentExpression(token));
                }
            }
            token.Current().Excepect(TokenType.Punctor, ")");
            token.Next();
            return args.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Refrences;
using torrent.Script.Stack;
using torrent.Script.Token;
using torrent.Script.Values;

namespace torrent.Script
{
    public class EnergyState
    {
        public VariabelDatabase VarDB = new VariabelDatabase();

        internal Statment ParseNext(Tokenizer token)
        {
            if (token.Current().Is("keyword"))
            {
                switch (token.Current().Context())
                {
                    case "function":
                        return this.GetFunction(token);
                    case "while":
                        return this.GetWhile(token);
                    case "if":
                        return this.GetIf(token);
                    case "return":
                        return this.GetReturn(token);
                    case "foreach":
                        return this.GetForeach(token);
                    case "for":
                        return this.GetFor(token);
                    case "global":
                        return this.GetGlobal(token);
                    case "delete":
                        return this.GetDelete(token);
                }
            }
            return this.HandleExpresion(token);
        }

        public Complication Evulate(Statment statment)
        {
            Complication com;
            Value value;
            Refrence re;
            switch (statment.Type())
            {
                case StatmentType.Function:
                    VariabelRefrence r = this.GetVariabel(statment.Name());
                    FunctionValue func = new FunctionValue(new FunctionScriptInstance(statment.Name(), statment.Args(), statment.Body(), this));
                    r.Put(func);
                    VariabelDatabaseValue v = r.GetAttribute();
                    v.IsGlobal = true;
                    v.IsLock = true;
                    return new Complication(ComplicationType.Normal, func);
                case StatmentType.Block:
                    Complication bl = new Complication(ComplicationType.Normal, new NullValue());
                    List<Statment> block = statment.Block();
                    for(int i = 0; i < block.Count; i++)
                    {
                        bl = this.Evulate(block[i]);
                        if (!bl.IsNormal())
                            return bl;
                    }
                    return bl;
                case StatmentType.Expresion:
                     return new Complication(ComplicationType.Normal, this.EvulateExpresion(statment.Expresion()));
                case StatmentType.While:
                    com = new Complication(ComplicationType.Normal, new NullValue());
                    while (this.EvulateExpresion(statment.Expresion()).ToBool())
                    {
                        com = this.Evulate(statment.Body());
                        if (!com.IsNormal())
                        {
                            if (com.IsReturn())
                                return com;
                            else
                            {
                                if (com.IsContinue())
                                {
                                    double number = com.GetValue().ToNumber() - 1;
                                    if (number != 0)
                                        return new Complication(ComplicationType.Continue, new NumberValue(number));
                                    com = new Complication(ComplicationType.Normal, new NullValue());
                                }else if (com.IsBreak())
                                {
                                    double number = com.GetValue().ToNumber() - 1;
                                    if (number != 0)
                                        return new Complication(ComplicationType.Break, new NumberValue(number));
                                    return new Complication(ComplicationType.Normal, new NullValue());
                                }
                            }
                        }
                    }
                    return com;
                case StatmentType.If:
                    if (this.EvulateExpresion(statment.Expresion()).ToBool())
                    {
                        return this.Evulate(statment.Body());
                    }

                    Statment s = statment.SecoundStatment();
                    if (s != null)
                        return this.Evulate(s);
                    return new Complication(ComplicationType.Normal, new NullValue());
                case StatmentType.Return:
                    return new Complication(ComplicationType.Return, ScriptUntil.GetValue(this.EvulateExpresion(statment.Expresion())));
                case StatmentType.Foreach:
                    value = this.EvulateExpresion(statment.Expresion());
                    if (value.Type() != "namedArray")
                        throw new ScriptRuntimeException("Foreach first argument must be a named array got: " + value.Type());
                    Dictionary<string, Value> name = (Dictionary<string, Value>)value.ToPrimtiv();
                    Complication fc = new Complication(ComplicationType.Normal, new NullValue());
                    foreach(string n in name.Keys)
                    {
                        this.GetVariabel(statment.Name()).Put(new StringValue(n));
                        if(statment.Value() != null)
                        {
                            this.GetVariabel(statment.Value()).Put(name[n]);
                        }

                        fc = this.Evulate(statment.Body());
                        if (!fc.IsNormal())
                        {
                            if (fc.IsReturn())
                            {
                                return fc;
                            }

                            if (fc.IsBreak())
                            {
                                int bn = (int)fc.GetValue().ToNumber() - 1;
                                if(bn != 0)
                                {
                                    return new Complication(ComplicationType.Break, new NumberValue(bn));
                                }
                                break;
                            }

                            if (fc.IsContinue())
                            {
                                int cn = (int)fc.GetValue().ToNumber() - 1;
                                if(cn != 0)
                                {
                                    return new Complication(ComplicationType.Continue, new NumberValue(cn));
                                }
                            }
                        }
                    }
                    return fc;
                case StatmentType.For:
                    Complication last = new Complication(ComplicationType.Normal, new NullValue());
                    EvulateExpresion(statment.Expresion());
                    while (EvulateExpresion(statment.SecondExpresion()).ToBool())
                    {
                        last = Evulate(statment.Body());
                        if (!last.IsNormal())
                        {
                            if (last.IsReturn())
                                return last;
                            else if (last.IsBreak())
                            {
                                int b = (int)last.GetValue().ToNumber() - 1;
                                if (b == 0)
                                    return new Complication(ComplicationType.Normal, new NullValue());
                                return new Complication(ComplicationType.Break, new NumberValue(b));
                            }else if (last.IsContinue())
                            {
                                int c = (int)last.GetValue().ToNumber() - 1;
                                if (c == 0)
                                    return new Complication(ComplicationType.Normal, new NullValue());
                                return new Complication(ComplicationType.Continue, new NumberValue(c));
                            }
                        }
                        ScriptUntil.GetValue(EvulateExpresion(statment.ThirdExpresion()));
                    }
                    return last;
                case StatmentType.Global:
                    re = EvulateExpresion(statment.Expresion()).ToRefrence();
                    if(!(re is VariabelRefrence))
                    {
                        throw new ScriptRuntimeException("Unknown result after 'global' keyword");
                    }
                    ((VariabelRefrence)re).GetAttribute().IsGlobal = true;
                    return new Complication(ComplicationType.Normal, new RefrenceValue(re));
                case StatmentType.Delete:
                    re = EvulateExpresion(statment.Expresion()).ToRefrence();
                    return new Complication(ComplicationType.Normal, new BoolValue(re.Delete()));
            }
            throw new ScriptParserException("Unknown statment detected '" + statment.Type() + "'. Parser out of sync");
        }

        private Value EvulateExpresion(Expresion expresion)
        {
            Value name = null;
            Refrence r;
            switch (expresion.Type)
            {
                case ExpresionType.Empty:
                    return new NullValue();
                case ExpresionType.Call:
                    Value v = this.EvulateExpresion(expresion.Left);
                    Value[] args = new Value[expresion.Args.Count];
                    for (int i = 0; i < expresion.Args.Count; i++)
                        args[i] = this.EvulateExpresion(expresion.Args[i]);
                    return v.ToFunction().Call(args);
                case ExpresionType.Identify:
                    return new RefrenceValue(this.GetVariabel(expresion.Name));
                case ExpresionType.String:
                    return new StringValue(expresion.Name);
                case ExpresionType.Assign:
                    name = this.EvulateExpresion(expresion.Left);
                    Value value = this.EvulateExpresion(expresion.Right);
                    name.ToRefrence().Put(value);
                    return name;
                case ExpresionType.Bool:
                    return new BoolValue(expresion.Name == "true");
                case ExpresionType.Not:
                    return new BoolValue(!this.EvulateExpresion(expresion.Left).ToBool());
                case ExpresionType.Equel:
                    return new BoolValue(ScriptUntil.Equeal(this.EvulateExpresion(expresion.Left), this.EvulateExpresion(expresion.Right)));
                case ExpresionType.NotEqual:
                    return new BoolValue(!ScriptUntil.Equeal(this.EvulateExpresion(expresion.Left), this.EvulateExpresion(expresion.Right)));
                case ExpresionType.Negetiv:
                    return new NumberValue(-this.EvulateExpresion(expresion.Left).ToNumber());
                case ExpresionType.Number:
                    return new NumberValue(Double.Parse(expresion.Name));
                case ExpresionType.AND:
                    return new BoolValue(this.EvulateExpresion(expresion.Left).ToBool() && this.EvulateExpresion(expresion.Right).ToBool());
                case ExpresionType.LOWER:
                    return new BoolValue(this.EvulateExpresion(expresion.Left).ToNumber() <= this.EvulateExpresion(expresion.Right).ToNumber());
                case ExpresionType.CREATOR:
                    return new BoolValue(this.EvulateExpresion(expresion.Left).ToNumber() >= this.EvulateExpresion(expresion.Right).ToNumber());
                case ExpresionType.Plus:
                    Value left = this.EvulateExpresion(expresion.Left);
                    Value right = this.EvulateExpresion(expresion.Right);
                    if (left.Type() != "number" || right.Type() != "number")
                        return new StringValue(left.toString() + right.toString());
                    return new NumberValue(left.ToNumber() + right.ToNumber());
                case ExpresionType.NamedArray:
                    Dictionary<string, Value> namedArray = new Dictionary<string, Value>();
                    foreach(Expresion key in expresion.NamedArray.Keys)
                    {
                        namedArray[this.EvulateExpresion(key).toString()] = this.EvulateExpresion(expresion.NamedArray[key]);
                    }
                    return new NamedArrayValue(namedArray);
                case ExpresionType.Array:
                    List<Value> array = new List<Value>();
                    List<Expresion> rawArray = expresion.Array;
                    for(int i = 0; i < rawArray.Count; i++)
                    {
                        array.Add(this.EvulateExpresion(rawArray[0]));
                    }
                    return new ArrayValue(array);
                case ExpresionType.ArrayGet:
                    Value get = ScriptUntil.GetValue(this.EvulateExpresion(expresion.Left));
                    if(expresion.Right != null)
                        name = this.EvulateExpresion(expresion.Right);
                    if (get.Type() == "namedArray")
                    {
                        return new RefrenceValue(new NamedArrayRefrence(name.toString(), (Dictionary<string, Value>)get.ToPrimtiv()));
                    }else if(get.Type() == "array")
                    {
                        if (name != null)
                            return new RefrenceValue(new ArrayRefrence(name, get.ToArray()));
                        return new RefrenceValue(new ArrayRefrence(get.ToArray()));
                    }
                    else
                    {
                        throw new ScriptRuntimeException("Cant use get operator on the type: "+get.Type());
                    }
                case ExpresionType.LEFT_ARROW:
                    return new BoolValue(EvulateExpresion(expresion.Left).ToNumber() < EvulateExpresion(expresion.Right).ToNumber());
                case ExpresionType.RIGHT_ARROW:
                    return new BoolValue(EvulateExpresion(expresion.Left).ToNumber() > EvulateExpresion(expresion.Right).ToNumber());
                case ExpresionType.SelfPlus:
                    r = EvulateExpresion(expresion.Left).ToRefrence();
                    r.Put(new NumberValue(r.Get().ToNumber() + EvulateExpresion(expresion.Right).ToNumber()));
                    return new RefrenceValue(r);
                case ExpresionType.SelfMinus:
                    r = EvulateExpresion(expresion.Left).ToRefrence();
                    r.Put(new NumberValue(r.Get().ToNumber() - EvulateExpresion(expresion.Right).ToNumber()));
                    return new RefrenceValue(r);
                case ExpresionType.Function:
                    return new FunctionValue(new FunctionScriptInstance("", expresion.FuncArgs, expresion.Body, this));
            }

            throw new ScriptRuntimeException("Unknown expresion type detected '" + expresion.Type + "'. Parser out of sync");
        }

        internal void pushVariabelStack(VariabelStack stack)
        {
            this.VarDB.Push(stack);
        }

        public void popVariabelStack()
        {
            this.VarDB.Pop();
        }

        private Statment HandleExpresion(Tokenizer token)
        {
            Statment statment;
            if (token.Current().Is("punctor", ";"))
                statment = new Statment(Expresion.Empty());
            else
                statment = new Statment(Expresion.Parse(token, this));
            token.Current().Expect("punctor", ";");
            token.Next();
            return statment;

        }

        private Statment GetDelete(Tokenizer token)
        {
            token.Next();
            Statment statment = new Statment(StatmentType.Delete, Expresion.Parse(token, this));
            token.Current().Expect("punctor", ";");
            token.Next();
            return statment;
        }

        private Statment GetGlobal(Tokenizer token)
        {
            token.Next();
            Expresion exp = Expresion.Parse(token, this);
            token.Current().Expect("punctor", ";");
            token.Next();
            return new Statment(StatmentType.Global, exp);
        }

        private Statment GetFor(Tokenizer token)
        {
            token.Next().Expect("punctor", "(");
            Expresion start   = Expresion.Empty();
            Expresion control = Expresion.Empty();
            Expresion end     = Expresion.Empty();

            if(!token.Next().Is("punctor", ";"))
            {
                start = Expresion.Parse(token, this);
            }

            token.Current().Expect("punctor", ";");
            
            if(!token.Next().Is("punctor", ";"))
            {
                control = Expresion.Parse(token, this);
            }

            token.Current().Expect("punctor", ";");

            if(!token.Next().Is("punctor", ")"))
            {
                end = Expresion.Parse(token, this);
            }

            token.Current().Expect("punctor", ")");

            return new Statment(StatmentType.For, start, control, end, this.GetBlock(token));
        }

        private Statment GetForeach(Tokenizer token)
        {
            token.Next().Expect("punctor", "(");
            token.Next();
            Expresion data = Expresion.Parse(token, this);
            token.Current().Expect("keyword", "as");
            string key = token.Next().Expect("identify").Context();
            string value = null;
            if(token.Next().Is("punctor", ":"))
            {
                value = token.Next().Expect("identify").Context();
                token.Next();
            }

            token.Current().Expect("punctor", ")");
            return new Statment(StatmentType.Foreach, data, key, value, this.GetBlock(token));
        }

        private Statment GetReturn(Tokenizer token)
        {
            Expresion expresion = Expresion.Empty();
            if (!token.Next().Is("punctor", ";"))
            {
                expresion = Expresion.Parse(token, this);
            }
            token.Current().Expect("punctor", ";");
            token.Next();
            return new Statment(StatmentType.Return, expresion);
        }

        private Statment GetIf(Tokenizer token)
        {
            token.Next().Expect("punctor", "(");
            token.Next();
            Expresion test = Expresion.Parse(token, this);
            token.Current().Expect("punctor", ")");
            Statment body = this.GetBlock(token);

            if (token.Current().Is("keyword", "elseif"))
            {
                return new Statment(StatmentType.If, test, body, this.GetIf(token));
            }else if(token.Current().Is("keyword", "else"))
            {
                return new Statment(StatmentType.If, test, body, this.GetBlock(token));
            }

            return new Statment(StatmentType.If, test, body);
        }

        private Statment GetWhile(Tokenizer token)
        {
            token.Next().Expect("punctor", "(");
            token.Next();
            Expresion controle = Expresion.Parse(token, this);
            token.Current().Expect("punctor", ")");
            Statment body = this.GetBlock(token);
            return new Statment(StatmentType.While, controle, body);
        }

        private Statment GetFunction(Tokenizer token)
        {
            string name = token.Next().Expect("identify").Context();
            token.Next().Expect("punctor", "(");
            List<string> args = new List<string>();
            if(!token.Next().Is("punctor", ")"))
            {
                args.Add(token.Current().Expect("identify").Context());
                token.Next();
                while(token.Current().Is("punctor", ","))
                {
                    args.Add(token.Next().Expect("identify").Context());
                    token.Next();
                }
            }

            token.Current().Expect("punctor", ")");
            Statment body = GetBlock(token);
            return new Statment(name, args, body);
        }

        internal VariabelRefrence GetVariabel(string name)
        {
            VariabelStack top = this.VarDB.Get(this.VarDB.Size() - 1);
            for(int i = this.VarDB.Size()-2; i >= 0; i--)
            {
                VariabelStack stack = this.VarDB.Get(i);
                if (stack.ContaineVariabel(name) && stack.GetAttribute(name).IsGlobal)
                {
                    return new VariabelRefrence(stack, name);
                }
            }
            return new VariabelRefrence(top, name);
        }
        

        public Statment GetBlock(Tokenizer token)
        {
            Statment statment;
            if(token.Next().Is("punctor", "{"))
            {
                token.Next();
                List<Statment> body = new List<Statment>();
                while (!token.Current().Is("punctor", "}"))
                    body.Add(this.ParseNext(token));
                statment = new Statment(body);
                token.Next();
            }
            else
            {
                statment = this.ParseNext(token);
            }
            return statment;
        }
    }
}

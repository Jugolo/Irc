using Irc.Script.Exceptions;
using Irc.Script.Types;
using Irc.Script.Types.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script
{
    static class EcmaEvulator
    {
        internal static EcmaComplication Evulate(EcmaState state, EcmaStatment statment)
        {
            EcmaComplication c = new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
            switch (statment.Type)
            {
                case EcmaStatmentType.FunctionDec:
                    return CreateFuncDec(state, statment);
                case EcmaStatmentType.Expresion:
                    return new EcmaComplication(EcmaComplicationType.Normal, EvulateExpresion(state, statment.Expresion));
                case EcmaStatmentType.Return:
                    return new EcmaComplication(EcmaComplicationType.Return, statment.Expresion == null ? EcmaValue.Undefined() : EvulateExpresion(state, statment.Expresion));
                case EcmaStatmentType.If:
                    if(Reference.GetValue(EvulateExpresion(state, statment.Expresion)).ToBoolean(state))
                    {
                        return Evulate(state, statment.Statment);
                    }
                    else
                    {
                        if(statment.Else == null)
                        {
                            return new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                        }
                        return Evulate(state, statment.Else);
                    }
                case EcmaStatmentType.Block:
                    for(int i = 0; i < statment.Statments.Count; i++)
                    {
                        c = Evulate(state, statment.Statments[i]);
                        if (c.Type != EcmaComplicationType.Normal)
                            return c;
                    }
                    return new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Null());
                case EcmaStatmentType.Var:
                    EcmaHeadObject putIn = state.GetScope()[state.GetScope().Length - 1];
                    foreach(string key in statment.VarList.Keys)
                    {
                        putIn.Put(key, statment.VarList[key] == null ? EcmaValue.Undefined() : Reference.GetValue(EvulateExpresion(state, statment.VarList[key])));
                    }
                    return new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                case EcmaStatmentType.While:
                    c = new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                    while(EvulateExpresion(state, statment.Expresion).ToBoolean(state))
                    {
                        c = Evulate(state, statment.Statment);
                        if(c.Type == EcmaComplicationType.Break)
                        {
                            return new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                        }else if(c.Type == EcmaComplicationType.Continue)
                        {
                            c = new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                        }else if(c.Type == EcmaComplicationType.Return)
                        {
                            return c;
                        }
                    }
                    return c;
                case EcmaStatmentType.For:
                    if(statment.Expresion != null)
                    {
                        Reference.GetValue(EvulateExpresion(state, statment.Expresion));
                    }
                    while(statment.Second == null || Reference.GetValue(EvulateExpresion(state, statment.Second)).ToBoolean(state))
                    {
                        c = Evulate(state, statment.Statment);
                        if(c.Type == EcmaComplicationType.Break)
                        {
                            return new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                        }else if(c.Type == EcmaComplicationType.Return)
                        {
                            return c;
                        }else if(c.Type == EcmaComplicationType.Continue)
                        {
                            c = new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                        }

                        if (statment.Tree != null)
                            Reference.GetValue(EvulateExpresion(state, statment.Tree));
                    }
                    return c;
                case EcmaStatmentType.ForIn:
                    EcmaHeadObject obj = Reference.GetValue(EvulateExpresion(state, statment.Second)).ToObject(state);
                    c = new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                    foreach(string key in obj.Property.Keys)
                    {
                        if (obj.Property[key].DontEnum)
                        {
                            continue;
                        }

                        if (statment.Expresion.Type == ExpresionType.VarList)
                            Reference.PutValue(EvulateExpresion(state, statment.Expresion.Multi[0].Left), EcmaValue.String(key), state.GlobalObject);
                        else
                            Reference.PutValue(EvulateExpresion(state, statment.Expresion), EcmaValue.String(key), state.GlobalObject);
                        c = Evulate(state, statment.Statment);
                        if(c.Type != EcmaComplicationType.Normal)
                        {
                            if (c.Type == EcmaComplicationType.Break)
                                return new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                            if (c.Type == EcmaComplicationType.Continue)
                                c = new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
                            if (c.Type == EcmaComplicationType.Return)
                                return c;
                        }
                    }
                    return c;
                case EcmaStatmentType.Continue:
                    return new EcmaComplication(EcmaComplicationType.Continue, EcmaValue.Undefined());
                default:
                    throw new EcmaRuntimeException("Evulator out of sync. Unknown statment type: " + statment.Type.ToString());
            }
        }

        private static EcmaValue EvulateExpresion(EcmaState state, ExpresionData expresion)
        {
            EcmaValue value = null;
            EcmaHeadObject obj;
            switch (expresion.Type)
            {
                case ExpresionType.MultiExpresion:
                    for (int i = 0; i < expresion.Multi.Count; i++)
                    {
                        value = Reference.GetValue(EvulateExpresion(state, expresion.Multi[i]));
                    }
                    return value;
                case ExpresionType.Assign:
                    if (expresion.Sign == "=")
                    {
                        EcmaValue ai = EvulateExpresion(state, expresion.Left);
                        value = Reference.GetValue(EvulateExpresion(state, expresion.Right));
                        Reference.PutValue(ai, value, state.GlobalObject);
                        return value;
                    }
                    else
                    {
                        EcmaValue sa = EvulateExpresion(state, expresion.Left);
                        value = Reference.GetValue(EvulateExpresion(state, expresion.Right));
                        value = EcmaMath.Math(state, Reference.GetValue(sa), expresion.Sign.Substring(0, 1), value);
                        Reference.PutValue(sa, value, state.GlobalObject);
                        return value;
                    }
                case ExpresionType.Conditional:
                    return Reference.GetValue(EvulateExpresion(state, expresion.Test)).ToBoolean(state) ? Reference.GetValue(EvulateExpresion(state, expresion.Left)) : Reference.GetValue(EvulateExpresion(state, expresion.Right));
                case ExpresionType.Or:
                    value = Reference.GetValue(EvulateExpresion(state, expresion.Left));
                    if (value.ToBoolean(state))
                        return value;
                    return Reference.GetValue(EvulateExpresion(state, expresion.Right));
                case ExpresionType.AND:
                    value = Reference.GetValue(EvulateExpresion(state, expresion.Left));
                    if (!value.ToBoolean(state))
                        return value;
                    return Reference.GetValue(EvulateExpresion(state, expresion.Right));
                case ExpresionType.BOR:
                    return EcmaMath.Math(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), "|", Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                case ExpresionType.XOR:
                    return EcmaMath.Math(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), "^", Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                case ExpresionType.BAND:
                    return EcmaMath.Math(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), "&", Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                case ExpresionType.Equlity:
                    bool er = EcmaEquel.IsEquel(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                    return EcmaValue.Boolean(expresion.Sign == "==" ? er : !er);
                case ExpresionType.Relational:
                    if (expresion.Sign == "<")
                    {
                        value = EcmaRelational.DoRelational(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                        if (value.IsUndefined())
                            return EcmaValue.Boolean(false);
                        return value;
                    }
                    else if (expresion.Sign == ">")
                    {
                        value = Reference.GetValue(EvulateExpresion(state, expresion.Left));
                        value = EcmaRelational.DoRelational(state, Reference.GetValue(EvulateExpresion(state, expresion.Right)), value);
                        if (value.IsUndefined())
                            return EcmaValue.Boolean(false);
                        return value;
                    }
                    else if (expresion.Sign == "<=")
                    {
                        value = EcmaRelational.DoRelational(state, Reference.GetValue(EvulateExpresion(state, expresion.Right)), Reference.GetValue(EvulateExpresion(state, expresion.Left)));
                        if (value.IsBoolean() && value.ToBoolean(state) || value.IsUndefined())
                            return EcmaValue.Boolean(false);
                        return EcmaValue.Boolean(true);
                    }
                    else if (expresion.Sign == ">=")
                    {
                        value = EcmaRelational.DoRelational(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                        if (value.IsBoolean() && value.ToBoolean(state) || value.IsUndefined())
                            return EcmaValue.Boolean(false);
                        return EcmaValue.Boolean(true);
                    }
                    return EcmaValue.Boolean(false);
                case ExpresionType.Shift:
                    return EcmaMath.Math(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), expresion.Sign, Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                case ExpresionType.Additive:
                    return EcmaMath.Math(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), expresion.Sign, Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                case ExpresionType.Multiplicative:
                    return EcmaMath.Math(state, Reference.GetValue(EvulateExpresion(state, expresion.Left)), expresion.Sign, Reference.GetValue(EvulateExpresion(state, expresion.Right)));
                case ExpresionType.Unary:
                    value = EvulateExpresion(state, expresion.Left);
                    switch (expresion.Sign)
                    {
                        case "delete":
                            return EcmaValue.Boolean(Reference.GetBase(value).Delete(Reference.GetPropertyName(value)));
                        case "void":
                            Reference.GetValue(value);
                            return EcmaValue.Undefined();
                        case "typeof":
                            if (value.IsRefrence() && Reference.GetBase(value) == null)
                            {
                                return EcmaValue.String("undefined");
                            }

                            value = Reference.GetValue(value);

                            if (value.IsObject())
                            {
                                obj = value.ToObject(state);
                                if (obj is ICallable)
                                {
                                    return EcmaValue.String("function");
                                }
                            }

                            return EcmaValue.String(value.Type().ToString().ToLower());
                        case "++":
                            double ul = Reference.GetValue(EvulateExpresion(state, expresion.Left)).ToNumber(state);
                            Reference.PutValue(value, EcmaValue.Number(ul + 1), state.GlobalObject);
                            return EcmaValue.Number(ul + 1);
                        case "--":
                            double nul = Reference.GetValue(EvulateExpresion(state, expresion.Left)).ToNumber(state);
                            Reference.PutValue(value, EcmaValue.Number(nul + 1), state.GlobalObject);
                            return EcmaValue.Number(nul + 1);
                        case "+":
                            return EcmaValue.Number(+Reference.GetValue(value).ToNumber(state));
                        case "-":
                            return EcmaValue.Number(-Reference.GetValue(value).ToNumber(state));
                        case "~":
                            return EcmaValue.Number(~Reference.GetValue(value).ToInt32(state));
                        case "!":
                            return EcmaValue.Boolean(!Reference.GetValue(value).ToBoolean(state));
                        default:
                            throw new EcmaRuntimeException("Unary evulation out of sync. Unknown sign: " + expresion.Sign);
                    }
                case ExpresionType.ItemGet:
                    obj = Reference.GetValue(EvulateExpresion(state, expresion.Left)).ToObject(state);
                    return EcmaValue.Reference(new Reference(
                        Reference.GetValue(EvulateExpresion(state, expresion.Right)).ToString(state),
                        obj
                        ));
                case ExpresionType.ObjGet:
                    return EcmaValue.Reference(new Reference(
                        expresion.Sign, 
                        Reference.GetValue(EvulateExpresion(state, expresion.Left)).ToObject(state)
                        ));
                case ExpresionType.New:
                    value = Reference.GetValue(EvulateExpresion(state, expresion.Left));
                    if (!value.IsObject())
                        throw new EcmaRuntimeException("After 'new' keyword there must be a object");
                    obj = value.ToObject(state);

                    if (!(obj is IConstruct))
                        throw new EcmaRuntimeException("Object dont implements Constructor");

                    value = (obj as IConstruct).Construct(GetArguments(state, obj, expresion.Arg));

                    if (!value.IsObject())
                        throw new EcmaRuntimeException("The constructor dont return a object");

                    return value;
                case ExpresionType.Call:
                    EcmaValue func = EvulateExpresion(state, expresion.Left);
                    value = Reference.GetValue(func);
                    obj = value.ToObject(state);
                    if (!(obj is ICallable))
                        throw new EcmaRuntimeException("The object dont implements Call");

                    EcmaHeadObject self;

                    if (func.IsRefrence())
                        self = Reference.GetBase(func);
                    else
                        self = null;

                    return (obj as ICallable).Call(self, GetArguments(state, obj, expresion.Arg));
                case ExpresionType.This:
                    return EcmaValue.Object(state.GetThis());
                case ExpresionType.Identify:
                    return EcmaValue.Reference(state.GetIdentify(expresion.Name));
                case ExpresionType.Number:
                    return EcmaValue.Number(Double.Parse(expresion.Sign));
                case ExpresionType.Null:
                    return EcmaValue.Undefined();
                case ExpresionType.Bool:
                    return EcmaValue.Boolean(expresion.Sign == "true");
                case ExpresionType.String:
                    return EcmaValue.String(expresion.Sign);
                case ExpresionType.VarList:
                    for (int i = 0; i < expresion.Multi.Count; i++)
                        value = EvulateExpresion(state, expresion.Multi[i]);
                    return value;
                default:
                    throw new EcmaRuntimeException("Evulator expresion out of sync. Unknown expresion type: " + expresion.Type);
            }
        }
        
        private static EcmaValue[] GetArguments(EcmaState state, EcmaHeadObject owner, ExpresionData[] args)
        {
            int length = owner.HasProperty("length") ? owner.Get("length").ToInt32(state) : 0;
            int i = 0;
            EcmaValue[] a = new EcmaValue[Math.Max(args.Length, length)];
            for (; i < args.Length; i++)
            {
                a[i] = Reference.GetValue(EvulateExpresion(state, args[i]));
            }

            for(; i < length; i++)
            {
                a[i] = EcmaValue.Undefined();
            }

            return a;
        }

        private static EcmaComplication CreateFuncDec(EcmaState state, EcmaStatment statment)
        {
            EcmaValue func = EcmaValue.Object(new FunctionInstance(state, statment.Args, statment.Statment));
            state.GlobalObject.Put(statment.Name, func);
            return new EcmaComplication(EcmaComplicationType.Normal, func);
        }
    }
}

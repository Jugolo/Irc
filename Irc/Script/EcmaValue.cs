using Irc.Script.Exceptions;
using Irc.Script.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    public class EcmaValue
    {
        public static EcmaValue Reference(Reference r)
        {
            return new EcmaValue(EcmaValueType.Refrence, r);
        }

        public static EcmaValue Undefined()
        {
            return new EcmaValue(EcmaValueType.Undefined, null);
        }

        public static EcmaValue Null()
        {
            return new EcmaValue(EcmaValueType.Null, null);
        }

        public static EcmaValue String(string context)
        {
            return new EcmaValue(EcmaValueType.String, context);
        }

        public static EcmaValue Object(EcmaHeadObject obj)
        {
            return new EcmaValue(EcmaValueType.Object, obj);
        }

        public static EcmaValue Boolean(bool context)
        {
            return new EcmaValue(EcmaValueType.Boolean, context);
        }

        public static EcmaValue Number(double number)
        {
            return new EcmaValue(EcmaValueType.Number, number);
        }

        private EcmaValueType type;
        private object value;

        private EcmaValue(EcmaValueType type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public EcmaValueType Type()
        {
            return this.type;
        }

        public bool IsObject()
        {
            return this.type == EcmaValueType.Object;
        }

        public bool IsPrimitiv()
        {
            return this.type != EcmaValueType.Object && this.type != EcmaValueType.Refrence;
        }

        public bool IsRefrence()
        {
            return this.type == EcmaValueType.Refrence;
        }

        public bool IsUndefined()
        {
            return this.type == EcmaValueType.Undefined;
        }

        public bool IsNull()
        {
            return this.type == EcmaValueType.Null;
        }

        public bool IsNumber()
        {
            return this.type == EcmaValueType.Number;
        }

        public bool IsString()
        {
            return this.type == EcmaValueType.String;
        }

        public bool IsBoolean()
        {
            return this.type == EcmaValueType.Boolean;
        }

        public Reference ToRefrence()
        {
            if (this.type == EcmaValueType.Refrence)
                return this.value as Reference;
            throw new EcmaRuntimeException("Cant convert " + this.type.ToString() + " to refrence");
        }

        public object ToPrimitiv(EcmaState state)
        {
            if (this.IsObject())
            {
                EcmaValue value = (this.value as EcmaHeadObject).DefaultValue(state);
                if(value.IsObject() || value.IsRefrence())
                {
                    throw new EcmaRuntimeException("Cant convert Object to primitiv value");
                }

                return value;
            }

            return this.value;
        }

        public int ToInt32(EcmaState state)
        {
            return (int)(uint)ToNumber(state);
        }

        public uint ToUint32(EcmaState state)
        {
            return (uint)ToNumber(state);
        }

        public double ToInteger(EcmaState state)
        {
            return (long)ToNumber(state);
        }

        public bool ToBoolean(EcmaState state)
        {
            switch (this.type)
            {
                case EcmaValueType.Undefined:
                case EcmaValueType.Null:
                    return false;
                case EcmaValueType.Boolean:
                    return (bool)this.value;
                case EcmaValueType.Number:
                    double number = this.ToNumber(state);
                    if (number == 0 || Double.IsNaN(number))
                        return false;
                    return true;
                case EcmaValueType.String:
                    return this.ToString(state).Length > 0;
                default:
                    return true;
            }
        }

        public double ToNumber(EcmaState state)
        {
            switch (this.type)
            {
                case EcmaValueType.Undefined:
                    return Double.NaN;
                case EcmaValueType.Null:
                    return 0.0;
                case EcmaValueType.Boolean:
                    return this.ToBoolean(state) ? 1.0 : 0.0;
                case EcmaValueType.Number:
                    return (double)this.value;
                case EcmaValueType.String:
                    return EcmaGramma.ConvertStringToNumber(this.ToString(state));
                case EcmaValueType.Object:
                    return (this.value as EcmaHeadObject).DefaultValue(state, "Number").ToNumber(state);
            }

            throw new EcmaRuntimeException("Cant convert " + this.type + " to Number");
        }

        public string ToString(EcmaState state)
        {
            switch (this.type)
            {
                case EcmaValueType.Undefined:
                    return "undefined";
                case EcmaValueType.Null:
                    return "null";
                case EcmaValueType.Boolean:
                    return this.ToBoolean(state) ? "true" : "false";
                case EcmaValueType.Number:
                    return EcmaGramma.ConvertStringToNumber(this.ToNumber(state));
                case EcmaValueType.String:
                    return (string)this.value;
                case EcmaValueType.Object:
                    return (this.value as EcmaHeadObject).DefaultValue(state, "String").ToString(state);
                default:
                    throw new EcmaRuntimeException("Cant convert " + this.type.ToString() + " to string");
            }
        }

        public EcmaHeadObject ToObject(EcmaState state)
        {
            EcmaValue[] arg = new EcmaValue[] { this };
            switch (this.type)
            {
                case EcmaValueType.Undefined:
                case EcmaValueType.Null:
                    throw new EcmaRuntimeException("Cant convert " + this.type.ToString() + " to Object");
                case EcmaValueType.Boolean:
                    //return (state.Boolean as IConstruct).Construct(arg).ToObject(state);
                    return new EcmaHeadObject();
                case EcmaValueType.Number:
                    //return (state.Number as IConstruct).Construct(arg).ToObject(state);
                    return new EcmaHeadObject();
                case EcmaValueType.String:
                    return (state.String as IConstruct).Construct(arg).ToObject(state);
                case EcmaValueType.Object:
                    return this.value as EcmaHeadObject;
                default:
                    throw new EcmaRuntimeException("Cant convert " + this.type.ToString() + " to Object");
            }
        }
    }
}

using Irc.Script.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    class EcmaMath
    {
        public static EcmaValue Math(EcmaState state, EcmaValue left, string sign, EcmaValue Right)
        {
            switch (sign)
            {
                case "|":
                    return BitwiseOr(state, left, Right);
                case "^":
                    return BitwiseXOR(state, left, Right);
                case "&":
                    return BitwiseAND(state, left, Right);
                case "<<":
                    return LeftShit(state, left, Right);
                case ">>":
                    return RightShift(state, left, Right);
                case ">>>":
                    return UnsignedRightShit(state, left, Right);
                case "+":
                    return Plus(state, left, Right);
                case "-":
                    return Minus(state, left, Right);
                case "*":
                    return Gange(state, left, Right);
                case "/":
                    return Divide(state, left, Right);
                case "%":
                    return Remainder(state, left, Right);
                default:
                    throw new EcmaRuntimeException("Unknown math sign: "+sign);
            }
        }

        public static EcmaValue Remainder(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToNumber(state) % right.ToNumber(state));
        }

        public static EcmaValue Divide(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToNumber(state) / right.ToNumber(state));
        }

        public static EcmaValue Gange(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToNumber(state) * right.ToNumber(state));
        }

        public static EcmaValue Minus(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToNumber(state) - right.ToNumber(state));
        }

        public static EcmaValue Plus(EcmaState state, EcmaValue left, EcmaValue right)
        {
            if(left.ToPrimitiv(state) is String || right.ToPrimitiv(state) is String)
            {
                return EcmaValue.String(left.ToString(state) + right.ToString(state));
            }

            return EcmaValue.Number(left.ToNumber(state) + right.ToNumber(state));
        }

        public static EcmaValue UnsignedRightShit(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number((uint)left.ToInt32(state) >> (int)(right.ToUint32(state) & 0x1F));
        }

        public static EcmaValue RightShift(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToInt32(state) >> right.ToInt32(state));
        }

        public static EcmaValue LeftShit(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToInt32(state) << right.ToInt32(state));
        }

        public static EcmaValue BitwiseAND(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToInt32(state) & right.ToInt32(state));
        }

        public static EcmaValue BitwiseXOR(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToInt32(state) ^ right.ToInt32(state));
        }

        public static EcmaValue BitwiseOr(EcmaState state, EcmaValue left, EcmaValue right)
        {
            return EcmaValue.Number(left.ToInt32(state) | right.ToInt32(state));
        }
    }
}

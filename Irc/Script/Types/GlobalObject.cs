using Irc.Script.Token;
using Irc.Script.Types.Array;
using Irc.Script.Types.Date;
using Irc.Script.Types.Function;
using Irc.Script.Types.Object;
using Irc.Script.Types.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script.Types
{
    public class GlobalObject : EcmaHeadObject
    {
        private EcmaState State;

        public GlobalObject(EcmaState state)
        {
            this.State = state;
            this.Put("Array", EcmaValue.Object(new ArrayConstructor(state)));
            this.Put("Object", EcmaValue.Object(new ObjectConstructor(state)));
            this.Put("String", EcmaValue.Object(new StringConstructor(state)));
            this.Put("Function", EcmaValue.Object(new FunctionConstructor(state)));
            this.Put("Date", EcmaValue.Object(new DateConstructor(state)));

            this.Put("parseFloat", EcmaValue.Object(new NativeFunctionInstance(1, state, ParseFloat)));
            this.Put("parseInt", EcmaValue.Object(new NativeFunctionInstance(1, state, ParseInt)));
        }

        private EcmaValue ParseInt(EcmaHeadObject obj, EcmaValue[] arg)
        {
            string inputString = arg[0].ToString(State);
            var s = inputString.TrimStart();

            var sign = 1;
            if (!System.String.IsNullOrEmpty(s))
            {
                if (s[0] == '-')
                {
                    sign = -1;
                }

                if (s[0] == '-' || s[0] == '+')
                {
                    s = s.Substring(1);
                }
            }

            var stripPrefix = true;

            int radix = arg.Length > 1 ? arg[1].ToInt32(State) : 0;

            if (radix == 0)
            {
                if (s.Length >= 2 && s.StartsWith("0x") || s.StartsWith("0X"))
                {
                    radix = 16;
                }
                else
                {
                    radix = 10;
                }
            }
            else if (radix < 2 || radix > 36)
            {
                return EcmaValue.Number(double.NaN);
            }
            else if (radix != 16)
            {
                stripPrefix = false;
            }

            if (stripPrefix && s.Length >= 2 && s.StartsWith("0x") || s.StartsWith("0X"))
            {
                s = s.Substring(2);
            }

            try
            {
                return EcmaValue.Number(sign * Parse(s, radix));
            }
            catch
            {
                return EcmaValue.Number(double.NaN);
            }
        }

        private EcmaValue ParseFloat(EcmaHeadObject obj, EcmaValue[] arg)
        {
            string input = arg[0].ToString(State).Trim();
            if (input.StartsWith("Infinity"))
            {
                return EcmaValue.Number(Double.PositiveInfinity);
            }

            StringBuilder builder = new StringBuilder();
            int i = 0;
            for(; i < input.Length; i++)
            {
                char c = input[i];
                if (!EcmaChar.IsDigit(c))
                    break;
                builder.Append(c);
            }

            if(i >= input.Length || input[i] != '.') 
            {
                if (builder.Length == 0)
                    return EcmaValue.Number(0);
                return EcmaValue.Number(Double.Parse(builder.ToString()));
            }

            builder.Append(".");
            i++;

            for (; i < input.Length; i++)
            {
                if (!EcmaChar.IsDigit(input[i]))
                    return EcmaValue.Number(Double.Parse(builder.ToString()));
                builder.Append(input[i]);
            }

            return EcmaValue.Number(Double.Parse(builder.ToString()));
        }

        private static double Parse(string number, int radix)
        {
            if (number == "")
            {
                return double.NaN;
            }

            double result = 0;
            double pow = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                double index = double.NaN;
                char digit = number[i];

                if (digit >= '0' && digit <= '9')
                {
                    index = digit - '0';
                }
                else if (digit >= 'a' && digit <= 'z')
                {
                    index = digit - 'a' + 10;
                }
                else if (digit >= 'A' && digit <= 'Z')
                {
                    index = digit - 'A' + 10;
                }

                if (double.IsNaN(index) || index >= radix)
                {
                    return Parse(number.Substring(0, i), radix);
                }

                result += index * pow;
                pow = pow * radix;
            }

            return result;
        }

    }
}

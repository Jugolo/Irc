using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    class EcmaGramma
    {
        public static double ConvertStringToNumber(string number)
        {
            if (number == null)
                return 0.0;

            number = number.Trim();
            if (number.Length == 0)
                return 0;

            if (number == "+Infinity" || number == "Infinity")
                return Double.PositiveInfinity;
            if (number == "-Infinity")
                return Double.NegativeInfinity;

            double d;
            if(Double.TryParse(number, out d))
            {
                return d;
            }

            return Double.NaN;
        }

        public static string ConvertStringToNumber(double number)
        {
            return number.ToString();
        }
    }
}

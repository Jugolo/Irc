using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script
{
    class EcmaRelational
    {
        public static EcmaValue DoRelational(EcmaState state, EcmaValue left, EcmaValue right)
        {
            object l = left.ToPrimitiv(state);
            object r = right.ToPrimitiv(state);

            if(l is String && r is String)
            {
                string ls = l as String;
                string rs = r as String;
                if (ls.IndexOf(rs) == 0)
                    return EcmaValue.Boolean(false);
                if (rs.IndexOf(ls) == 0)
                    return EcmaValue.Boolean(true);
                char[] lc = ls.ToCharArray();
                char[] rc = rs.ToCharArray();

                for(int i=0;i<Math.Min(ls.Length, rs.Length); i++)
                {
                    if (lc[i] > rc[i])
                        return EcmaValue.Boolean(false);
                }

                return EcmaValue.Boolean(true);
            }

            double x = left.ToNumber(state);
            double y = right.ToNumber(state);

            if (Double.IsNaN(x) || Double.IsNaN(y))
                return EcmaValue.Undefined();

            if (x == y)
                return EcmaValue.Boolean(false);

            if (x == -0 && y == +0 || x == +0 && y == -0)
                return EcmaValue.Boolean(false);

            if (Double.IsPositiveInfinity(x) || Double.IsNegativeInfinity(y))
                return EcmaValue.Boolean(false);

            if (Double.IsNegativeInfinity(x) || Double.IsPositiveInfinity(y))
                return EcmaValue.Boolean(true);
            
            return EcmaValue.Boolean(y > x);
        }
    }
}

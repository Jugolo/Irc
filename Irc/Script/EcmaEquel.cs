using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    class EcmaEquel
    {
        public static bool IsEquel(EcmaState state, EcmaValue x, EcmaValue y)
        {
            if(x.Type() == y.Type())
            {
                if (x.IsUndefined() || x.IsNull())
                    return true;

                if (x.IsNumber())
                {
                    double xn = x.ToNumber(state);
                    double yn = y.ToNumber(state);

                    if (Double.IsNaN(xn) || Double.IsNaN(yn))
                        return false;

                    if (xn == +0 && yn == -0 || xn == -0 && yn == +0)
                        return true;
                    return xn == yn;
                }

                if (x.IsString())
                    return x.ToString(state) == y.ToString(state);

                if (x.IsBoolean())
                    return x.ToBoolean(state) == y.ToBoolean(state);

                return y.ToObject(state) == x.ToObject(state);
            }

            if (x.IsNull() && y.IsUndefined() || x.IsUndefined() && y.IsUndefined())
                return true;

            if (!(x.IsNumber() || x.IsString()) && y.IsObject() || x.IsObject() && !(y.IsNumber() || y.IsString()))
                return x.ToPrimitiv(state) == y.ToPrimitiv(state);
            return x.ToNumber(state) == y.ToNumber(state);
        }
    }
}

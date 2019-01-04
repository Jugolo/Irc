using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    enum ExpresionType
    {
        MultiExpresion,
        Identify,
        Assign,
        String,
        Conditional,
        Or,
        AND,
        BOR,
        XOR,
        BAND,
        Equlity,
        Relational,
        Shift,
        Additive,
        Multiplicative,
        Unary,
        New,
        ObjGet,
        ItemGet,
        Call,
        This,
        Null,
        Bool,
        Number,
        VarList
    }
}

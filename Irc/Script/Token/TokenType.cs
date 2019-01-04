using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Token
{
    public enum TokenType
    {
        Identify,
        Punctor,
        String,
        Number,
        EOS,
        Keyword,
        Null,
        Bool
    }
}

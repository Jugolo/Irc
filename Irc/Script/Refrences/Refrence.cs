using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Values;

namespace torrent.Script.Refrences
{
    public interface Refrence
    {
        void Put(Value value);
        Value Get();
        bool Delete();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Stack;
using torrent.Script.Values;

namespace torrent.Script
{
    public interface FunctionInstance
    {
        Value Call(Value[] args);
        String Name();
    }
}

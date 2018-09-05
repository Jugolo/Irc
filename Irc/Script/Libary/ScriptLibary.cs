using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script.Error;
using torrent.Script.Stack;
using torrent.Script.Values;

namespace torrent.Script.Libary
{
    class ScriptLibary
    {

        public static void Init(Energy energy)
        {
            VariabelDatabaseValue v = energy.PutValue("include", new FunctionValue(new FunctionNativeInstance("include", Include)));
            v.IsLock = true;
            v.IsGlobal = true;
        }

        private static Value Include(Value[] args)
        {
            string arg = args[0].toString();
            if (arg.IndexOf("System.") == 0)
                return GetSystemArg(arg);
            Energy e = new Energy();
            e.Parse(File.OpenText("Script/"+arg));
            Complication c = e.GetLast();
            if (c == null || !c.IsReturn())
                throw new ScriptRuntimeException("A included file must return a value");
            return c.GetValue();
        }

        private static Value GetSystemArg(string arg)
        {
            switch (arg)
            {
                //System function
                case "System.Alert":
                    return new FunctionValue(new FunctionNativeInstance("@System.Alert", LibSystem.Alert));
                //File functions
                case "System.IO.File.Exists":
                    return new FunctionValue(new FunctionNativeInstance("@System.Io.File.Exists", LibaryFile.GetExists));
                case "System.IO.File.Create":
                    return new FunctionValue(new FunctionNativeInstance("@System.IO.File.Create", LibaryFile.GetCreate));
                case "System.IO.File.GetContents":
                    return new FunctionValue(new FunctionNativeInstance("@System.IO.File.GetContents", LibaryFile.GetGetContents));
                case "System.IO.File.PutContents":
                    return new FunctionValue(new FunctionNativeInstance("@System.IO.File.PutContents", LibaryFile.GetPutContents));
                //Types function
                case "System.Type":
                    return new FunctionValue(new FunctionNativeInstance("@System.Type", LibaryType.GetType));
                case "System.Type.Numric":
                    return new FunctionValue(new FunctionNativeInstance("@System.Type.Numric", LibaryType.GetNumric));
                case "System.Type.ToNumber":
                    return new FunctionValue(new FunctionNativeInstance("@System.Type.ToNumber", LibaryType.GetToNumber));
                //Array function
                case "System.Array.Count":
                    return new FunctionValue(new FunctionNativeInstance("@System.Array.Count", LibaryArray.GetCount));
                //String function
                case "System.String.Length":
                    return new FunctionValue(new FunctionNativeInstance("@System.Array.Length", LibaryString.Length));
                case "System.String.CharArray":
                    return new FunctionValue(new FunctionNativeInstance("@System.String.CharArray", LibaryString.CharArray));
                case "System.String.Strpos":
                    return new FunctionValue(new FunctionNativeInstance("@System.String.Strpos", LibaryString.Strpos));
                case "System.String.Substr":
                    return new FunctionValue(new FunctionNativeInstance("@System.String.Substr", LibaryString.Substr));
                default:
                    throw new ScriptRuntimeException("Unknown system libary detected: " + arg);
            }
        }
    }
}

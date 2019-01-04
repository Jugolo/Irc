using Irc.Script;
using Irc.Script.Exceptions;
using Irc.Script.Types;
using Irc.Script.Types.Function;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc
{
    class DefaultScript
    {
        private EcmaState state;

        public DefaultScript(EcmaScript e)
        {
            state = e.State;
            e.CreateVariable("include", EcmaValue.Object(new NativeFunctionInstance(1, state, Include)));
        }

        public EcmaValue Include(EcmaHeadObject self, EcmaValue[] arg)
        {
            string a = arg[0].ToString(state);
            switch (a)
            {
                case "System.Class":
                    return EcmaValue.Object(new NativeFunctionInstance(1, state, GetClass));
                case "System.Alert":
                    return EcmaValue.Object(new NativeFunctionInstance(1, state, Alert));
                case "System.IO.File.GetContents":
                    return EcmaValue.Object(new NativeFunctionInstance(1, state, GetFileContents));
                case "System.IO.File.PutContents":
                    return EcmaValue.Object(new NativeFunctionInstance(2, state, PutFileContents));
                case "System.Hash.Sha1":
                    return EcmaValue.Object(new NativeFunctionInstance(1, state, Sha1));
                case "System.IO.File":
                    return GetFil();
                case "System.Encoding.Ben":
                    return LoadScriptFile("Include/Encoding/Ben.js");
                default:
                    return LoadScriptFile("Script/" + a);
            }
        }

        private EcmaValue GetFil()
        {
            EcmaHeadObject fil = new EcmaHeadObject();

            fil.Put("exists", EcmaValue.Object(new NativeFunctionInstance(1, state, FileExists)));
            fil.Put("create", EcmaValue.Object(new NativeFunctionInstance(1, state, FileCreate)));
            fil.Put("getContent", EcmaValue.Object(new NativeFunctionInstance(1, state, FileGetContent)));
            fil.Put("writeContents", EcmaValue.Object(new NativeFunctionInstance(2, state, FileWriteContents)));
            fil.Put("writeLine", EcmaValue.Object(new NativeFunctionInstance(2, state, FileWriteLine)));

            return EcmaValue.Object(fil);
        }

        private EcmaValue FileWriteLine(EcmaHeadObject self, EcmaValue[] arg)
        {
            string file = arg[0].ToString(state);
            if (File.Exists(file))
            {
                FileStream stream = File.OpenWrite(file);
                byte[] bytes = Encoding.ASCII.GetBytes(arg[1].ToString(state));
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
            return EcmaValue.Boolean(false);
        }

        private EcmaValue FileWriteContents(EcmaHeadObject self, EcmaValue[] arg)
        {
            string file = arg[0].ToString(state);
            if (!File.Exists(file))
                return EcmaValue.Boolean(false);

            File.WriteAllText(file, arg[1].ToString(state));
            return EcmaValue.Boolean(true);
        }

        private EcmaValue FileGetContent(EcmaHeadObject self, EcmaValue[] arg)
        {
            string file = arg[0].ToString(state);
            if (File.Exists(file))
                return EcmaValue.String(File.ReadAllText(file));
            return EcmaValue.Null();
        }

        private EcmaValue FileCreate(EcmaHeadObject self, EcmaValue[] args)
        {
            File.Create(args[0].ToString(state)).Close();
            return EcmaValue.Undefined();
        }

        private EcmaValue LoadScriptFile(string path)
        {
            if (!File.Exists(path))
                throw new EcmaRuntimeException("Unknown script path: " + path);

            EcmaScript script = new EcmaScript();
            script.BuildStandartLibary();
            new DefaultScript(script);
            EcmaComplication com = script.RunCode(File.OpenText(path));
            if (com.Type != EcmaComplicationType.Return)
            {
                throw new EcmaRuntimeException("A included file must return a value!");
            }

            return com.Value;
        }

        private EcmaValue Sha1(EcmaHeadObject self, EcmaValue[] arg)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(arg[0].ToString(state)));
                StringBuilder sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return EcmaValue.String(sb.ToString());
            }
        }

        private EcmaValue GetClass(EcmaHeadObject self, EcmaValue[] args)
        {
            return EcmaValue.String(args[0].ToObject(state).Class);
        }

        public EcmaValue PutFileContents(EcmaHeadObject self, EcmaValue[] args)
        {
            string path = args[0].ToString(state);
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            File.WriteAllText(path, args[1].ToString(state));
            return EcmaValue.Null();
        }

        public EcmaValue GetFileContents(EcmaHeadObject self, EcmaValue[] args)
        {
            string path = args[0].ToString(state);
            if (File.Exists(path))
            {
                return EcmaValue.String(File.ReadAllText(path));
            }

            return EcmaValue.Null();
        }

        public EcmaValue FileExists(EcmaHeadObject self, EcmaValue[] args)
        {
            return EcmaValue.Boolean(File.Exists(args[0].ToString(state)));
        }

        public EcmaValue Alert(EcmaHeadObject self, EcmaValue[] args)
        {
            MessageBox.Show(args[0].ToString(state));
            return EcmaValue.Undefined();
        }
    }
}

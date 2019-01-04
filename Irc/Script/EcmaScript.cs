using Irc.Script.Exceptions;
using Irc.Script.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    class EcmaScript
    {
        public EcmaState State { get; private set; }

        public EcmaScript()
        {
            this.State = new EcmaState();
        }

        public void BuildStandartLibary()
        {
            if (this.State.InitedStandartLibary)
                throw new EcmaRuntimeException("You can`t reinvoke standart libary");
            this.State.BuildStandartLibary();
        }

        public EcmaComplication RunCode(string code)
        {
            return RunCode(new StringReader(code));
        }

        public EcmaComplication RunCode(TextReader reader)
        {
            EcmaComplication result = new EcmaComplication(EcmaComplicationType.Normal, EcmaValue.Undefined());
            EcmaTokenizer token = new EcmaTokenizer(reader);
            while (token.Current().IsNot(TokenType.EOS))
            {
                result = EcmaEvulator.Evulate(this.State, this.State.GetProgroam(token));
            }
            return result;
        }

        public void CreateVariable(string name, EcmaValue value)
        {
            Reference.PutValue(this.State.GetIdentify(name), value, State.GlobalObject);
        }

        public EcmaValue GetVariabel(string name)
        {
            return Reference.GetValue(EcmaValue.Reference(this.State.GetIdentify(name)));
        }
    }
}

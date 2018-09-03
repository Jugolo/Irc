using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script;
using torrent.Script.Error;
using torrent.Script.Stack;
using torrent.Script.Values;

namespace Irc.Irc
{
    class IrcScript
    {
        private Energy energy = new Energy();
        private IrcConection connection;
        private Form1 main;

        public IrcScript(IrcConection connection, Form1 main)
        {
            this.connection = connection;
            this.main = main;

            this.pushFunction(new FunctionNativeInstance("on", On));
            this.pushFunction(new FunctionNativeInstance("action", _Action));
            this.pushFunction(new FunctionNativeInstance("privmsg", Privmsg));
            this.pushFunction(new FunctionNativeInstance("channel", Channel));
            this.pushFunction(new FunctionNativeInstance("nick", Nick));
            this.pushFunction(new FunctionNativeInstance("rawMessage", RawMessage));
            this.pushFunction(new FunctionNativeInstance("myNick", MyNick));

            energy.Parse(new InputTextReader(File.OpenText("script.txt")));
        }

        private Value _Action(Value[] args)
        {
            this.connection.scriptAction.Add(args[0].toString(), args[1].ToFunction());
            return new NullValue();
        }

        private Value MyNick(Value[] args)
        {
            return new StringValue(this.connection.GetNick());
        }

        private Value On(Value[] args)
        {
            this.connection.scriptEvenets.Add(args[0].toString(), args[1].ToFunction());
            return new NullValue();
        }

        private void pushFunction(FunctionInstance func)
        {
            VariabelDatabaseValue v = this.energy.PutValue(func.Name(), new FunctionValue(func));
            v.IsGlobal = true;
            v.IsLock = true;
        }

        private Value Privmsg(Value[] args)
        {
            this.connection.SendLine("PRIVMSG " + args[0].toString() + " :" + args[1].toString());
            this.connection.Flush();
            this.main.channel1.ShowLine(this.connection.GetIdentify(), args[0].toString(), this.connection.GetNick(), args[1].toString());
            
            return new NullValue();
        }

        private Value Channel(Value[] args)
        {
            IrcScriptMessage msg = this.EnsureScriptMessage(args[0]);
            return new StringValue(IrcUntil.GetChannel(msg.message));
        }

        private Value Nick(Value[] args)
        {
            return new StringValue(this.EnsureScriptMessage(args[0]).message.Nick);
        }

        private Value RawMessage(Value[] args)
        {
            return new StringValue(this.EnsureScriptMessage(args[0]).message.Raw);
        }

        private IrcScriptMessage EnsureScriptMessage(Value value)
        {
            value = ScriptUntil.GetValue(value);
            if(!(value is IrcScriptMessage))
            {
                throw new ScriptRuntimeException("Cant convert " + value.Type() + " to irc message stream");
            }

            return (IrcScriptMessage)value;
        }
    }
}

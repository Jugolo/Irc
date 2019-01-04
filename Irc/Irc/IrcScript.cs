using Irc.Irc.IS;
using Irc.Script;
using Irc.Script.Types;
using Irc.Script.Types.Function;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Irc
{
    public class IrcScript
    {
        private EcmaScript energy = new EcmaScript();
        private IrcConection connection;
        private Form1 main;

        public EcmaState State { get { return this.energy.State; } }

        public IrcScript(IrcConection connection, Form1 main)
        {
            this.energy.BuildStandartLibary();
            new DefaultScript(this.energy);
            this.connection = connection;
            this.main = main;

            this.energy.CreateVariable("_identify", EcmaValue.String(connection.GetIdentify()));
            this.energy.CreateVariable("sendAction", EcmaValue.Object(new NativeFunctionInstance(2, State, SendAction)));

            this.energy.CreateVariable("on", EcmaValue.Object(new NativeFunctionInstance(2, State, On)));
            this.energy.CreateVariable("action", EcmaValue.Object(new NativeFunctionInstance(2, State, _Action)));
            this.energy.CreateVariable("privmsg", EcmaValue.Object(new NativeFunctionInstance(2, State, Privmsg)));
            this.energy.CreateVariable("myNick", EcmaValue.Object(new NativeFunctionInstance(0, State, MyNick)));
            this.energy.CreateVariable("isPm", EcmaValue.Object(new NativeFunctionInstance(1, State, IsPM)));

            this.energy.CreateVariable("join", EcmaValue.Object(new NativeFunctionInstance(1, State, Join)));
            this.energy.CreateVariable("whois", EcmaValue.Object(new NativeFunctionInstance(1, State, Whois)));
            this.energy.CreateVariable("sleep", EcmaValue.Object(new NativeFunctionInstance(1, State, Sleep)));
            
            MysqlScript.Init(energy);

            energy.RunCode(new StreamReader(File.OpenRead("script.js")));
        }

        public EcmaValue Sleep(EcmaHeadObject obj, EcmaValue[] args)
        {
            Thread.Sleep(args[0].ToInt32(this.energy.State));
            return EcmaValue.Null();
        }

        public EcmaValue Whois(EcmaHeadObject obj, EcmaValue[] whois)
        {
            this.connection.SendLine("WHOIS " + whois[0].ToString(this.energy.State));
            this.connection.Flush();
            return EcmaValue.Null();
        }

        public EcmaValue Join(EcmaHeadObject obj, EcmaValue[] args)
        {
            this.connection.SendLine("JOIN " + Reference.GetValue(args[0]).ToString(this.energy.State));
            this.connection.Flush();
            return EcmaValue.Null();
        }

        public EcmaValue SendAction(EcmaHeadObject obj, EcmaValue[] args)
        {
            this.connection.SendLine("PRIVMSG " + args[0].ToString(this.energy.State) + " :" + '\x001'.ToString() + "ACTION " + args[1].ToString(this.energy.State));
            this.connection.Flush();
            return EcmaValue.Null();
        }

        private EcmaValue IsPM(EcmaHeadObject obj, EcmaValue[] args)
        {
            return EcmaValue.Boolean(args[0].ToString(this.energy.State).IndexOf("#") != 0);
        }

        private EcmaValue _Action(EcmaHeadObject obj, EcmaValue[] args)
        {
            if (!this.connection.scriptAction.ContainsKey(args[0].ToString(this.energy.State)))
                this.connection.scriptAction.Add(args[0].ToString(this.energy.State), args[1].ToObject(this.State));
            return EcmaValue.Null();
        }

        private EcmaValue MyNick(EcmaHeadObject obj, EcmaValue[] args)
        {
            return EcmaValue.String(this.connection.GetNick());
        }

        private EcmaValue On(EcmaHeadObject obj, EcmaValue[] args)
        {
            if(!this.connection.scriptEvenets.ContainsKey(args[0].ToString(this.energy.State)))
                this.connection.scriptEvenets.Add(args[0].ToString(this.energy.State), args[1].ToObject(this.energy.State));
            return EcmaValue.Null();
        }

        private EcmaValue Privmsg(EcmaHeadObject obj, EcmaValue[] args)
        {
            this.connection.SendLine("PRIVMSG " + args[0].ToString(this.energy.State) + " :" + args[1].ToString(this.energy.State));
            this.connection.Flush();
            this.main.channel1.Write(this.connection.GetIdentify(), args[0].ToString(this.energy.State), this.connection.GetNick(), args[1].ToString(this.energy.State));

            return EcmaValue.Null();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using torrent.Script;
using torrent.Script.Values;

namespace Irc.Database
{
    public class Config
    {
        private Energy script = new Energy();
        public string Identify { get; private set; }


        public Config(string identify)
        {
            this.Identify = identify;
            this.script.PutValue("identify", new StringValue(identify));
            this.script.Parse(new InputTextReader(File.OpenText("Script/Database/Config.txt")));
            if (this.script.HasValue("loadConfig"))
            {
                this.script.GetVariabel("loadConfig").ToFunction().Call(new Value[0]);
            }
        }

        public String this[string key]
        {
            get
            {
                if (this.script.HasValue("get"))
                {
                    return this.script.GetVariabel("get").ToFunction().Call(new Value[] { new StringValue(key) }).toString();
                }
                return "";
            }

            set
            {
                if (this.script.HasValue("set"))
                {
                    this.script.GetVariabel("set").ToFunction().Call(new Value[] {
                        new StringValue(key),
                        new StringValue(value)
                    });
                }
            }
        }
    }
}

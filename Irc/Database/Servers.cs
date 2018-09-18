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
    public class Servers
    {
        public delegate void GetData(string identify, string host, int port, string nick, string[] channels);
        private Energy energy;

        public Servers()
        {
            energy = new Energy();
            energy.Parse(File.OpenText("Script/Database/Server.txt"));

        }

        public int GetServerCount()
        {
            if (energy.HasValue("getListCount"))
                return (int)energy.GetVariabel("getListCount").ToFunction().Call(new Value[0]).ToNumber();
            throw new Exception("A script function missing. getListCount()");
        }

        public void Save(string identify, string host, int port, string nick, string[] channels)
        {
            if (energy.HasValue("saveServer"))
                energy.GetVariabel("saveServer").ToFunction().Call(new Value[] 
                {
                    new StringValue(identify),
                    new StringValue(host),
                    new NumberValue(port),
                    new StringValue(nick),
                    new ArrayValue(ToStringArray(channels))
                });
            else
                throw new Exception("Mising saveServer in database server script");
        }

        public void UpdateNick(string identify, string nick)
        {
            if (this.energy.HasValue("updateNick"))
            {
                this.energy.GetVariabel("udateNick").ToFunction().Call(new Value[]{
                    new StringValue(identify),
                    new StringValue(nick)
                    });
            }
            else
                throw new Exception("Missing updateNick in server script database file");
        }

        public void SaveChannel(string identify, string channel)
        {
            if (this.energy.HasValue("saveChannel"))
            {
                energy.GetVariabel("saveChannel").ToFunction().Call(new Value[] {
                    new StringValue(identify),
                    new StringValue(channel)
                });
            }
            else
            {
                throw new Exception("Missing saveChannel in database server script");
            }
        }

        public void DeleteConnection(string identify)
        {
            if (energy.HasValue("deleteServer"))
            {
                energy.GetVariabel("deleteServer").ToFunction().Call(new Value[]
                {
                    new StringValue(identify)
                });
            }
            else
                throw new Exception("Server database script missing 'deleteServer(identify)' function");
        }

        public void DeleteChannel(string identify, string channel)
        {
            if (energy.HasValue("deleteChannel"))
            {
                energy.GetVariabel("deleteChannel").ToFunction().Call(new Value[]
                {
                    new StringValue(identify),
                    new StringValue(channel)
                });
            }
            else
                throw new Exception("Server database script missing 'deleteChannel(identify, channel)' function");
        }

        public void ForeachServers(GetData callback)
        {
            if (energy.HasValue("getServer"))
            {
                int count = this.GetServerCount();
                for(int i = 0; i < count; i++)
                {
                    Value v = energy.GetVariabel("getServer").ToFunction().Call(new Value[]
                    {
                        new NumberValue(i)
                    });
                    Dictionary<string, Value> result = v.ToNamedArray();
                    callback(
                        result["identify"].toString(),
                        result["host"].toString(),
                        (int)result["port"].ToNumber(),
                        result["nick"].toString(),
                        FromStringArray(result["channels"].ToArray())
                        );
                }
            }
            else
                throw new Exception("Missing getServer(int i) in database server script");
        }

        private List<Value> ToStringArray(string[] items)
        {
            List<Value> result = new List<Value>();
            for (int i = 0; i < items.Length; i++)
                result.Add(new StringValue(items[i]));
            return result;
        }

        private string[] FromStringArray(List<Value> value)
        {
            string[] result = new string[value.Count];
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = value[i].toString();
            }
            return result;
        }
    }
}

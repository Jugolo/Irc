using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Irc.Script.Types;
using Irc.Script;
using Irc.Script.Types.Function;

namespace Irc.Irc.IS
{
    class MysqlScript
    {
        public static void Init(EcmaScript energy)
        {
            EcmaHeadObject obj = new EcmaHeadObject();
            obj.Class = "MysqlConnector";
            obj.Put("connect", EcmaValue.Object(new NativeFunctionInstance(4, energy.State, (EcmaHeadObject o, EcmaValue[] arg) =>
            {
                MySqlConnection connection = new MySqlConnection("SERVER=" + arg[0].ToString(energy.State) + ";UID=" + arg[1].ToString(energy.State) + ";PASSWORD=" + arg[2].ToString(energy.State) + ";DATABASE=" + arg[3].ToString(energy.State) + ";");
                try
                {
                    connection.Open();
                    return EcmaValue.Object(new MysqlValue(energy.State, connection));
                }
                catch (MySqlException)
                {
                    return EcmaValue.Null();
                }
            })));
            energy.CreateVariable("MysqlConnector", EcmaValue.Object(obj));

            obj = new EcmaHeadObject();
            obj.Class = "Mysql";
            obj.Put("escape", EcmaValue.Object(new NativeFunctionInstance(1, energy.State, (EcmaHeadObject o, EcmaValue[] arg) =>
            {
                return EcmaValue.String(MySqlHelper.EscapeString(arg[0].ToString(energy.State)));
            })));
        }
    }
}

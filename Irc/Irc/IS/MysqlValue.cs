using Irc.Script;
using Irc.Script.Types;
using Irc.Script.Types.Function;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc.IS
{
    class MysqlValue : EcmaHeadObject
    {
        private MySqlConnection connection;
        private EcmaState state;

        public MysqlValue(EcmaState state, MySqlConnection connection)
        {
            this.state = state;
            this.connection = connection;
            this.Put("query", EcmaValue.Object(new NativeFunctionInstance(0, state, Query)));
        }

        private EcmaValue Query(EcmaHeadObject obj, EcmaValue[] arg)
        {
            string query = arg[0].ToString(this.state);
            DataTable table = new DataTable();
            MySqlDataReader command = new MySqlCommand(query, this.connection).ExecuteReader();
            table.Load(command);
            command.Close();
            return EcmaValue.Object(new MysqlReaderValue(state, table));
        }
    }
}

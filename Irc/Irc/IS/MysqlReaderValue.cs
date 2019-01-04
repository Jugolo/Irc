using Irc.Script;
using Irc.Script.Types;
using Irc.Script.Types.Function;
using System.Data;
using System.Linq;

namespace Irc.Irc.IS
{
    class MysqlReaderValue : EcmaHeadObject
    {
        private DataRow[] reader;
        public int Current { get; private set; }

        public MysqlReaderValue(EcmaState state, DataTable reader)
        {
            this.Current = 0;
            this.reader = reader.AsEnumerable().ToArray();
            this.Put("fetch", EcmaValue.Object(new NativeFunctionInstance(0, state, Fetch)));
        }

        private EcmaValue Fetch(EcmaHeadObject obj, EcmaValue[] arg)
        {
            if(this.reader.Count() < this.Current)
            {
                return EcmaValue.Null();
            }

            DataRow row = this.reader[this.Current];
            this.Current++;
            EcmaHeadObject result = new EcmaHeadObject();
            for(int i = 0; i < row.ItemArray.Length; i++)
            {
                EcmaValue c = EcmaValue.String(row[i].ToString());
                result.Put(i.ToString(), c);
                result.Put(row.Table.Columns[i].ColumnName, c);
            }

            return EcmaValue.Object(result);
        }
    }
}

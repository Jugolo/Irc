using Irc.Script.Exceptions;
using Irc.Script.Types;
using Irc.Script.Types.Array;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    class EcmaUntil
    {
        public static EcmaHeadObject ToArray(EcmaState state, List<Object> item)
        {
            ArrayIntstance array = new ArrayIntstance(state, new EcmaValue[0]);
            for(int i = 0; i < item.Count; i++)
            {
                if (item[i] is EcmaHeadObject)
                    array.Put(i.ToString(), EcmaValue.Object(item[i] as EcmaHeadObject));
                else if (item[i] is String)
                    array.Put(i.ToString(), EcmaValue.String(item[i] as String));
                else if (item[i] is Boolean)
                    array.Put(i.ToString(), EcmaValue.Boolean((bool)item[i]));
                else if (item[i] is Double)
                    array.Put(i.ToString(), EcmaValue.Number((double)item[i]));
                else if (item[i] == null)
                    array.Put(i.ToString(), EcmaValue.Null());
                else
                    throw new EcmaRuntimeException("Could not convert " + item[i].GetType().FullName + " to ecma value");

            }
            return array;
        }

        public static EcmaValue ToValue(object value)
        {
            if(value is String)
            {
                return EcmaValue.String(value as String);
            }
            throw new EcmaRuntimeException("Cant convert " + value.GetType().Name);
        }

        public static string[] ToStringArray(EcmaState state, EcmaHeadObject obj)
        {
            string[] result = new string[obj.Get("length").ToInt32(state)];
            for (int i = 0; i < result.Length; i++)
                result[i] = obj.Get(i.ToString()).ToString(state);
            return result;
        }
    }
}

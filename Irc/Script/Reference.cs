using Irc.Script.Exceptions;
using Irc.Script.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script
{
    public class Reference
    {
        public static EcmaHeadObject GetBase(EcmaValue value)
        {
            if (value.IsRefrence())
            {
                return value.ToRefrence().Base;
            }

            throw new EcmaRuntimeException("Cant get base of non refrence");
        }

        public static string GetPropertyName(EcmaValue value)
        {
            if (value.IsRefrence())
                return value.ToRefrence().Name;
            throw new EcmaRuntimeException("Cant get propertyname of non refrence");
        }

        public static EcmaValue GetValue(EcmaValue value)
        {
            if (!value.IsRefrence())
                return value;

            EcmaHeadObject b = GetBase(value);

            if(b == null)
            {
                throw new EcmaRuntimeException("Unkown identify "+GetPropertyName(value));
            }

            return b.Get(GetPropertyName(value));
        }

        public static void PutValue(Reference V, EcmaValue W, EcmaHeadObject Global)
        {
            EcmaHeadObject obj = V.Base;
            if(obj != null)
            {
                obj.Put(V.Name, W);
            }
            else
            {
                Global.Put(V.Name, W);
            }
        }

        public static void PutValue(EcmaValue V, EcmaValue W, EcmaHeadObject Global)
        {
            if (!V.IsRefrence())
                throw new EcmaRuntimeException("Cant put value on non refrence");

            EcmaHeadObject obj = GetBase(V);
            if (obj != null)
            {
                obj.Put(GetPropertyName(V), W);
            }
            else
            {
                Global.Put(GetPropertyName(V), W);
            }
        }

        public EcmaHeadObject Base { get; private set; }
        public string Name { get; private set; }

        public Reference(string Name)
        {
            this.Name = Name;
        }

        public Reference(string Name, EcmaHeadObject Base)
        {
            this.Name = Name;
            this.Base = Base;
        }
    }
}

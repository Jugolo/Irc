using Irc.Script.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script.Types
{
    public class EcmaHeadObject
    {
        public Dictionary<string, EcmaProperty> Property = new Dictionary<string, EcmaProperty>();
        public EcmaHeadObject Prototype { get; set; }
        public string Class { get; set; }
        public EcmaValue Value { get; set; }

        public virtual EcmaValue Get(string P)
        {
            if (this.Property.ContainsKey(P))
            {
                return this.Property[P].Value;
            }

            if(this.Prototype != null)
            {
                return this.Prototype.Get(P);
            }

            return EcmaValue.Undefined();
        }

        public virtual void Put(string P, EcmaValue V)
        {
            if (!this.CanPut(P))
                return;

            if (this.Property.ContainsKey(P))
            {
                this.Property[P].Value = V;
                return;
            }

            EcmaProperty prop = new EcmaProperty();
            prop.Value = V;

            this.Property.Add(P, prop);
        }

        public bool HasProperty(string P)
        {
            if (this.Property.ContainsKey(P))
                return true;

            if (this.Prototype != null)
                return this.Prototype.HasProperty(P);

            return false;
        }

        public bool Delete(string P)
        {
            if (!this.Property.ContainsKey(P))
                return true;

            if (this.Property[P].DontDelete)
                return false;

            this.Property.Remove(P);
            return true;
        }

        public EcmaValue DefaultValue(EcmaState state)
        {
            return DefaultValue(state, "Number");
        }

        public EcmaValue DefaultValue(EcmaState state, string hint)
        {
            EcmaValue result;
            EcmaHeadObject obj;
            if(hint == "String")
            {
                result = this.Get("toString");
                if (result.IsObject())
                {
                    obj = result.ToObject(state);
                    if (obj is ICallable)
                    {
                        result = (obj as ICallable).Call(this, new EcmaValue[0]);
                        if (result.IsPrimitiv())
                            return result;
                    }
                }

                result = this.Get("valueOf");
                if (result.IsObject())
                {
                    obj = result.ToObject(state);
                    if (obj is ICallable)
                    {
                        result = (obj as ICallable).Call(this, new EcmaValue[0]);
                        if (result.IsPrimitiv())
                            return result;
                    }
                }

                throw new EcmaRuntimeException("Cant get defualt value 'String' out of " + this.Class+" this class: "+this.GetType().FullName);
            }

            result = this.Get("valueOf");
            if (result.IsObject())
            {
                obj = result.ToObject(state);
                if (obj is ICallable)
                {
                    result = (obj as ICallable).Call(this, new EcmaValue[0]);
                    if (result.IsPrimitiv())
                        return result;
                }
            }

            result = this.Get("toString");
            if (result.IsObject())
            {
                obj = result.ToObject(state);
                if (obj is ICallable)
                {
                    result = (obj as ICallable).Call(this, new EcmaValue[0]);
                    if (result.IsPrimitiv())
                        return result;
                }
            }

            throw new EcmaRuntimeException("Cant convert " + this.Class + " to Number");
        }

        
        protected bool CanPut(string P)
        {
            if (this.Property.ContainsKey(P))
            {
                return !this.Property[P].ReadOnly;
            }

            if(this.Prototype != null)
            {
                return this.Prototype.CanPut(P);
            }

            return true;
        }
    }
}

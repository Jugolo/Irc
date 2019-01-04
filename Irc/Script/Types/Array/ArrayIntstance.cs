using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Script.Types.Array
{
    class ArrayIntstance : EcmaHeadObject
    {
        private EcmaState state;

        public ArrayIntstance(EcmaState state, EcmaValue[] values)
        {
            this.state = state;
            this.Prototype = state.Array;
            this.Class = "Array";
           
            if (values.Length == 1 && values[0].IsNumber())
            {
                EcmaProperty p = new EcmaProperty();
                p.Value = EcmaValue.Number(values[0].ToNumber(state));
                p.DontEnum = true;
                p.DontDelete = true;
                this.Property.Add("length", p);
            }
            else
            {
                EcmaProperty p = new EcmaProperty();
                p.Value = EcmaValue.Number(values.Length);
                p.DontEnum = true;
                p.DontDelete = true;
                this.Property.Add("length", p);
                for (int i = 0; i < values.Length; i++)
                {
                    this.Put(i.ToString(), values[i]);
                }
            }
        }

        public override void Put(string P, EcmaValue V)
        {
            if(P == "length")
            {
                if(V.ToInt32(state) < this.Get("length").ToInt32(state))
                {
                    foreach(string key in Property.Keys)
                    {
                        int current;
                        if(int.TryParse(key, out current) && current > V.ToInt32(state))
                        {
                            Property.Remove(key);
                        }
                    }
                }
                this.Property["length"].Value = EcmaValue.Number(V.ToNumber(state));
            }
            else
            {
                int current; 
                if(int.TryParse(P, out current))
                {
                    if(current >= this.Get("length").ToUint32(state))
                    {
                        this.Property["length"].Value = EcmaValue.Number(current + 1);
                    }
                }
                base.Put(P, V);
            }
        }
    }
}

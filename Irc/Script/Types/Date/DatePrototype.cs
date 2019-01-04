using Irc.Script.Exceptions;
using Irc.Script.Types.Function;
using System;
using System.Text;
using System.Windows.Forms;

namespace Irc.Script.Types.Date
{
    public class DatePrototype : EcmaHeadObject
    {

        public DatePrototype(EcmaState state, DateConstructor constructor)
        {
            this.Class = "Date";
            this.Value = EcmaValue.Number(Double.NaN);
            this.Prototype = state.Object;
            this.Put("constructor", EcmaValue.Object(constructor));
            this.Put("toString", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                AssetDate(self);
                return ConvertToString(state, self);
            })));
            this.Put("valueOf", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) =>
            {
                AssetDate(self);
                return ConvertToString(state, self);
            })));
            this.Put("getTime", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) =>
            {
                AssetDate(self);
                return self.Value;
            })));
            this.Put("getYear", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) =>
            {
                double year = self.Value.ToInteger(state);
                if (Double.IsNaN(year))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.YearFromTime(DateFunc.LocalTime(year)) - 1900);
            })));
            this.Put("getFullYear", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) =>
            {
                double year = self.Value.ToInteger(state);
                if (Double.IsNaN(year))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.YearFromTime(DateFunc.LocalTime(year)));
            })));
            this.Put("getUTCFullYear", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double year = self.Value.ToInteger(state);
                if (Double.IsNaN(year))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.YearFromTime(year));
            })));
            this.Put("getMonth", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double month = self.Value.ToInteger(state);
                if(Double.IsNaN(month))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.MonthFromTime(DateFunc.LocalTime(month)));
            })));
            this.Put("getUTCMonth", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) =>
            {
                double month = self.Value.ToInteger(state);
                if (Double.IsNaN(month))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.MonthFromTime(month));
            })));
            this.Put("getDate", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.DateFromTime(DateFunc.LocalTime(t)));
            })));
            this.Put("getUTCDate", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) =>
            {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.DateFromTime(t));
            })));
            this.Put("getDay", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.WeekDay(DateFunc.LocalTime(t)));
            })));
            this.Put("getUTCDay", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) =>
            {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.WeekDay(t));
            })));
            this.Put("getHours", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.HourFromTime(DateFunc.LocalTime(t)));
            })));
            this.Put("getUTCHours", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.HourFromTime(t));
            })));
            this.Put("getMinutes", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t)) 
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.MinFromTime(DateFunc.LocalTime(t)));
            })));
            this.Put("getUTCMinutes", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.MinFromTime(t));
            })));
            this.Put("getSeconds", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) =>
            {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.SecFromTime(DateFunc.LocalTime(t)));
            })));
            this.Put("getUTCSeconds", EcmaValue.Object(new NativeFunctionInstance(0, state, (self, arg) => {
                double t = self.Value.ToInteger(state);
                if (Double.IsNaN(t))
                    return EcmaValue.Number(Double.NaN);
                return EcmaValue.Number(DateFunc.SecFromTime(t));
            })));
        }

        public double Parse(string str)
        {
            DateTime now = DateTime.UtcNow;

            double year = now.Year;
            double months = now.Month;
            double date = now.Day;
            double hour = now.Hour;
            double minuts = now.Minute;
            double seconds = now.Second;
            double ms = now.Millisecond;
            double buffer;

            if (str.IndexOf("/") != -1){
                string[] dateParts = str.Split('/');
                if(dateParts.Length == 3)
                {
                    if(Double.TryParse(dateParts[0], out buffer))
                    {
                        year = buffer;
                    }

                    if(Double.TryParse(dateParts[1], out buffer))
                    {
                        months = buffer;
                    }

                    if(dateParts[2].IndexOf(":") != -1)
                    {
                        int pos = dateParts[2].IndexOf(' ');
                        string sm = dateParts[2].Substring(0, pos);
                        if(Double.TryParse(sm, out buffer))
                        {
                            date = buffer;
                        }

                        dateParts = dateParts[2].Substring(pos + 1).Split(':');

                        if(Double.TryParse(dateParts[0], out buffer))
                        {
                            hour = buffer;
                        }

                        if(Double.TryParse(dateParts[1], out buffer))
                        {
                            minuts = buffer;
                        }

                        if(Double.TryParse(dateParts[2], out buffer))
                        {
                            seconds = buffer;
                        }

                        if(Double.TryParse(dateParts[3], out buffer))
                        {
                            ms = buffer;
                        }
                    }
                    else
                    {
                        if(Double.TryParse(dateParts[2], out buffer))
                        {
                            date = buffer;
                        }
                    }
                }
            }
            return DateFunc.MakeDate(
                            DateFunc.MakeDay(
                                year,
                                months,
                                date
                                ),
                            DateFunc.MakeTime(
                                hour,
                                minuts,
                                seconds,
                                ms
                                )
                            );
        }

        private static EcmaValue ConvertToString(EcmaState state, EcmaHeadObject obj)
        {
            DateTime time = new DateTime() + TimeSpan.FromMilliseconds(obj.Value.ToInteger(state));
            StringBuilder builder = new StringBuilder();
            builder.Append(time.Year + "/" + time.Month + "/" + time.Day + " " + time.Hour + ":" + time.Minute + ":" + time.Second + ":" + time.Millisecond);
            return EcmaValue.String(builder.ToString());
        }

        private static void AssetDate(EcmaHeadObject obj)
        {
            if (obj.Class != "Date")
                throw new EcmaRuntimeException("Cant call this function without the object is a Date object");
        }
    }
}
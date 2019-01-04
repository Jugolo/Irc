using Irc.Script.Types.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Script.Types.Date
{
    public class DateConstructor : EcmaHeadObject, ICallable, IConstruct
    {
        public EcmaState State;
        public DateTime Begin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public DatePrototype DatePrototype;

        public DateConstructor(EcmaState state)
        {
            this.State = state;
            this.Put("length", EcmaValue.Number(7));
            this.Prototype = state.Function;
            this.DatePrototype = new DatePrototype(state, this);

            this.Put("prototype", EcmaValue.Object(this.DatePrototype));
            this.Property["prototype"].DontDelete = true;
            this.Property["prototype"].DontEnum = true;
            this.Property["prototype"].ReadOnly = true;

            this.Put("parse", EcmaValue.Object(new NativeFunctionInstance(
                1,
                this.State,
                (self, arg) =>
                {
                    return EcmaValue.Object(new DateInstance(this, DatePrototype.Parse(arg[0].ToString(State))));
                }
                )));
            this.Put("UTC", EcmaValue.Object(new NativeFunctionInstance(
                7,
                this.State,
                (self, arg) =>
                {
                    if (arg[0].IsUndefined())
                    {
                        return EcmaValue.Number(
                            TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds
                            );
                    }

                    double year = arg[0].ToInteger(State);
                    double month = arg[1].IsUndefined() ? 1 : arg[1].ToInteger(State);
                    double date = arg[2].IsUndefined() ? 1 : arg[2].ToInteger(State);
                    double hours = arg[3].IsUndefined() ? 0 : arg[3].ToInteger(State);
                    double minutes = arg[4].IsUndefined() ? 0 : arg[4].ToInteger(State);
                    double seconds = arg[5].IsUndefined() ? 0 : arg[5].ToInteger(State);
                    double ms = arg[6].IsUndefined() ? 0 : arg[6].ToInteger(State);

                    if(!Double.IsNaN(year) && 0 <= year && year <= 99)
                    {
                        year += 1900;
                    }

                    return EcmaValue.Number(
                        DateFunc.MakeDate(
                            DateFunc.MakeDay(
                                year,
                                month,
                                date
                                ),
                            DateFunc.MakeTime(
                                hours,
                                minutes,
                                seconds,
                                ms
                                )
                            )
                        );
                }
                )));

        }

        public EcmaValue Call(EcmaHeadObject self, EcmaValue[] arg)
        {
            return this.Construct(new EcmaValue[]{
                EcmaValue.Undefined(),
                EcmaValue.Undefined(),
                EcmaValue.Undefined(),
                EcmaValue.Undefined(),
                EcmaValue.Undefined(),
                EcmaValue.Undefined(),
                EcmaValue.Undefined(),
                });
        }

        public EcmaValue Construct(EcmaValue[] arg)
        {
            
            if (arg[0].IsUndefined())
                return EcmaValue.Object(new DateInstance(this, TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds));

            if (arg[1].IsUndefined())
            {
                double a1;
                if (arg[0].IsString())
                    a1 = DatePrototype.Parse(arg[0].ToString(this.State));
                else
                    a1 = arg[0].ToInteger(this.State);

                return EcmaValue.Object(new DateInstance(this, DateFunc.TimeClip(a1)));
            }

            double year = arg[0].ToInteger(this.State);
            double month = arg[1].ToInteger(this.State);
            double date = 1;
            double hours = 0;
            double minutes = 0;
            double seconds = 0;
            double ms = 0;

            if (!arg[2].IsUndefined())
                date = arg[2].ToInteger(this.State);

            if (!arg[3].IsUndefined())
                hours = arg[3].ToInteger(this.State);

            if (!arg[4].IsUndefined())
                minutes = arg[4].ToInteger(this.State);

            if (!arg[5].IsUndefined())
                seconds = arg[5].ToInteger(this.State);

            if (!arg[6].IsUndefined())
                ms = arg[6].ToInteger(this.State);

            if (!Double.IsNaN(year) && 0 <= year && year <= 99)
            {
                year += 1900;
            }

            return EcmaValue.Object(new DateInstance(this,
                    DateFunc.TimeClip(
                        DateFunc.UTC(
                            DateFunc.MakeDate(
                                DateFunc.MakeDay(
                                    year,
                                    month,
                                    date
                                    ),
                                DateFunc.MakeTime(
                                    hours,
                                    minutes,
                                    seconds,
                                    ms
                                    )
                                )
                            )
                        )
                    ));
        }
    }
}

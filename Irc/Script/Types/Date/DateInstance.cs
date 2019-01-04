using System;

namespace Irc.Script.Types.Date
{
    internal class DateInstance : EcmaHeadObject
    {

        public DateInstance(DateConstructor constructor, double time)
        {
            this.Value = EcmaValue.Number(time);
            this.Prototype = constructor.DatePrototype;
            this.Class = "Date";

        }


    }
}
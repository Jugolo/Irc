using Irc.Script.Exceptions;
using System;
using System.Windows.Forms;

namespace Irc.Script.Types.Date
{
    public class DateFunc
    {
        public const int HoursPerDay = 24;
        public const int MinutesPerHour = 60;
        public const int SecondsPerMinute = 60;

        public const int msPerDay = 86400000;
        public const int msPerSecond = 1000;
        public const int msPerMinute = msPerSecond * SecondsPerMinute;
        public const int msPerHour = msPerMinute * MinutesPerHour;

        public static double HourFromTime(double t)
        {
            return Math.Floor(t / msPerHour) % HoursPerDay;
        }

        public static double MinFromTime(double t)
        {
            return Math.Floor(t / msPerMinute) % MinutesPerHour;
        }

        public static double SecFromTime(double t)
        {
            return Math.Floor(t / msPerSecond) % SecondsPerMinute;
        }

        public static double Day(double t)
        {
            return (double)Math.Floor(t / msPerDay);
        }

        public static double TimeWithinDay(int t)
        {
            return t % msPerDay;
        }

        public static int DaysInYear(double y)
        {
            if(y % 4 != 0)
            {
                return 365;
            }

            if (y % 4 == 0 && y % 100 != 0)
                return 366;

            if (y % 100 == 0 && y % 400 != 0)
                return 365;

            if (y % 400 == 0)
                return 366;

            throw new EcmaRuntimeException("Failed to find DaysInYear");
        }

        public static double DayFromYear(double y)
        {
            return 365 * (y - 1970) + Math.Floor((y - 1969) / 4) - Math.Floor((y - 1901) / 100) + Math.Floor((y - 1601) / 400);
        }

        public static double TimeFromYear(double y)
        {
            return msPerDay * DayFromYear(y);
        }

        public static double YearFromTime(double t)
        {
            if (!AreFinite(t))
            {
                return double.NaN;
            }

            int sign = (t < 0) ? -1 : 1;
            int year = (sign < 0) ? 1969 : 1970;
            for (double timeToTimeZero = t; ;)
            {
                //  Subtract the current year's time from the time that's left.
                double timeInYear = DaysInYear(year) * msPerDay;
                timeToTimeZero -= sign * timeInYear;

                //  If there's less than the current year's worth of time left, then break.
                if (sign < 0)
                {
                    if (sign * timeToTimeZero <= 0)
                    {
                        break;
                    }
                    else
                    {
                        year += sign;
                    }
                }
                else
                {
                    if (sign * timeToTimeZero < 0)
                    {
                        break;
                    }
                    else
                    {
                        year += sign;
                    }
                }
            }

            return year;
        }

        public static int InLeapYear(double t)
        {
            return DaysInYear(YearFromTime(t)) == 366 ? 1 : 0;
        }

        public static double DayWithinYear(double t)
        {
            MessageBox.Show(Day(t)+" - "+ YearFromTime(t));
            return Day(t) - DayFromYear(YearFromTime(t));
        }

        public static double MonthFromTime(double t)
        {
            double dwy = DayWithinYear(t);
            int leap = InLeapYear(t);

            if (0 <= dwy && dwy < 31)
                return 0;

            if (31 <= dwy && dwy < 59 + leap)
                return 1;

            if (59 + leap <= dwy && dwy < 90 + leap)
                return 2;

            if (90 + leap <= dwy && dwy < 120 + leap)
                return 3;

            if (120 + leap <= dwy && dwy < 151 + leap)
                return 4;

            if (151 + leap <= dwy && dwy < 181 + leap)
                return 5;

            if (181 + leap <= dwy && dwy < 212 + leap)
                return 6;

            if (212 + leap <= dwy && dwy < 243 + leap)
                return 7;

            if (243 + leap <= dwy && dwy < 273 + leap)
                return 8;

            if (273 + leap <= dwy && dwy < 304 + leap)
                return 9;

            if (304 + leap <= dwy && dwy < 334 + leap)
                return 10;

            if (334 + leap <= dwy && dwy < 365 + leap)
                return 11;

            throw new EcmaRuntimeException("Could not find month"+dwy);
        }

        public static double DateFromTime(double t)
        {
            switch (MonthFromTime(t))
            {
                case 0:
                    return DayWithinYear(t) + 1;
                case 1:
                    return DayWithinYear(t) - 30;
                case 2:
                    return DayWithinYear(t) - 58 - InLeapYear(t);
                case 3:
                    return DayWithinYear(t) - 89 - InLeapYear(t);
                case 4:
                    return DayWithinYear(t) - 119 - InLeapYear(t);
                case 5:
                    return DayWithinYear(t) - 150 - InLeapYear(t);
                case 6:
                    return DayWithinYear(t) - 180 - InLeapYear(t);
                case 7:
                    return DayWithinYear(t) - 211 - InLeapYear(t);
                case 8:
                    return DayWithinYear(t) - 242 - InLeapYear(t);
                case 9:
                    return DayWithinYear(t) - 272 - InLeapYear(t);
                case 10:
                    return DayWithinYear(t) - 303 - InLeapYear(t);
                case 11:
                    return DayWithinYear(t) - 333 - InLeapYear(t);
                default:
                    return 0;

            }
        }

        public static double WeekDay(double t)
        {
            return (Day(t) + 4) % 7;
        }

        public static double DaylightSavingTA(double t)
        {
            return msPerDay;
        }

        public static double LocalTime(double t)
        {
            return t + DaylightSavingTA(t);
        }

        public static double UTC(double t)
        {
            return t - DaylightSavingTA(t);
        }

        public static double MakeTime(double hour, double min, double sec, double ms)
        {
            if (!AreFinite(hour, min, ms))
                return Double.NaN;

            return hour * msPerHour + min * msPerMinute + sec * msPerSecond + ms;
        }

        public static double MakeDay(double year, double month, double date)
        {

            if (!AreFinite(year, month, date))
                return Double.NaN;

            if (month < 0)
            {
                long mo = (long)month;
                year += (mo - 11) / 12;
                month = (12 + mo % 12) % 12;
            }

            int sign = (year < 1970) ? -1 : 1;
            double t = (year < 1970) ? 1 : 0;
            int yy;

            if (sign == -1)
            {
                for (yy = 1969; yy >= year; yy += sign)
                {
                    t += sign * DaysInYear(year) * msPerDay;
                }
            }
            else
            {
                for (yy = 1970; yy < year; yy += sign)
                {
                    t += sign * DaysInYear(year) * msPerDay;
                }
            }

            for (int mo = 0; mo < month; mo++)
            {
                t += DaysInMonth(month, InLeapYear(t)) * msPerDay;
            }

            return Day(t) + date - 1;
        }

        public static double MakeDate(double day, double time)
        {
            if (!AreFinite(day, time))
                return Double.NaN;

            return day * msPerDay + time;
        }

        public static double TimeClip(double time)
        {
            if (!AreFinite(time))
                return Double.NaN;

            if (Math.Abs(time) > 8.64e15)
                return Double.NaN;

            return time + (+0);
        }

        public static double DaysInMonth(double month, double leap)
        {
            month = month % 12;

            switch ((long)month)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                case 7:
                case 9:
                case 11:
                    return 31;
                case 3:
                case 5:
                case 8:
                case 10:
                    return 30;
                case 1:
                    return 28 + leap;
                default:
                    return 0;
            }
        }

        private static bool AreFinite(params double[] values)
        {
            for (int index = 0; index < values.Length; index++)
            {
                var value = values[index];
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
using System;

namespace Qi
{
    /// <summary>
    /// 
    /// </summary>
    public struct Time
    {
        private TimeSpan _ticks;

        public Time(int hour, int mins, int second)
        {
            _ticks = CheckTimeSpanBound(new TimeSpan(hour, mins, second));
        }

        public Time(int hour, int mins, int second, int millsecond)
        {
            _ticks = CheckTimeSpanBound(new TimeSpan(0, hour, mins, second, millsecond));
        }

        public Time(long ticks)
        {
            _ticks = CheckTimeSpanBound(new TimeSpan(ticks));
        }

        public long Ticks
        {
            get { return _ticks.Ticks; }
        }

        public int Hours
        {
            get { return _ticks.Hours; }
        }

        public int Minutes
        {
            get { return _ticks.Minutes; }
        }

        public int Seconds
        {
            get { return _ticks.Seconds; }
        }

        public int Millseconds
        {
            get { return _ticks.Milliseconds; }
        }

        public Time AddHours(int hour)
        {
            var result = new Time(_ticks.Add(new TimeSpan(hour, 0, 0)).Ticks);
            return CheckBound(result._ticks);
        }

        public Time AddMinutes(int min)
        {
            var result = new Time(_ticks.Add(new TimeSpan(0, min, 0)).Ticks);
            return CheckBound(result._ticks);
        }

        public Time AddSeconds(int second)
        {
            var result = new Time(_ticks.Add(new TimeSpan(0, 0, second)).Ticks);
            return CheckBound(result._ticks);
        }

        public Time AddMillseconds(int millsecond)
        {
            var result = new Time(_ticks.Add(new TimeSpan(0, 0, 0, millsecond)).Ticks);
            return CheckBound(result._ticks);
        }


        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}", Hours, Minutes, Seconds);
        }

        public string ToString(string format)
        {
            var time = new DateTime(_ticks.Ticks);
            return time.ToString(format);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var time = new DateTime(_ticks.Ticks);
            return time.ToString(format, formatProvider);
        }


        private static TimeSpan CheckTimeSpanBound(TimeSpan time)
        {
            if (time.Days >= 1)
            {
                //int subtractDay = Convert.ToInt32(time.Days - 1);
                var s = new TimeSpan(time.Days, 0, 0, 0);
                return time - s;
            }
            return time;
        }

        private static Time CheckBound(TimeSpan time)
        {
            return new Time(CheckTimeSpanBound(time).Ticks);
        }


        public static TimeSpan operator -(Time a, Time b)
        {
            return a._ticks - b._ticks;
        }

        public static DateTime operator -(DateTime dt, Time t1)
        {
            long tick = dt.Ticks - t1._ticks.Ticks;
            return new DateTime(tick);
        }

        public static TimeSpan operator +(Time a, Time b)
        {
            return a._ticks + b._ticks;
        }

        public static DateTime operator +(DateTime dt, Time t1)
        {
            long tick = dt.Ticks + t1._ticks.Ticks;
            return new DateTime(tick);
        }

        public static DateTime operator +(Time t1, DateTime dt)
        {
            return dt + t1;
        }

        public static bool operator ==(Time a, Time b)
        {
            return a._ticks == b._ticks;
        }

        public static bool operator !=(Time a, Time b)
        {
            return !(a == b);
        }

        public static bool operator >=(Time a, Time b)
        {
            return a._ticks >= b._ticks;
        }

        public static bool operator <=(Time a, Time b)
        {
            return a._ticks <= b._ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >(Time a, Time b)
        {
            return a._ticks > b._ticks;
        }

        public static bool operator <(Time a, Time b)
        {
            return a._ticks < b._ticks;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _ticks.GetHashCode() * 3;
            }
        }

        public override bool Equals(object obj)
        {
            bool result = base.Equals(obj);
            if (result)
                return true;
            if (obj.GetType() != GetType())
                return true;

            return ((Time)obj) == this;
        }
    }
}
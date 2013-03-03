using System;

namespace Qi
{
    /// <summary>
    /// 
    /// </summary>
    public struct Time
    {
        private readonly TimeSpan _ticks;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="mins"></param>
        /// <param name="second"></param>
        public Time(int hour, int mins, int second)
        {
            _ticks = CheckTimeSpanBound(new TimeSpan(hour, mins, second));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="mins"></param>
        /// <param name="second"></param>
        /// <param name="millsecond"></param>
        public Time(int hour, int mins, int second, int millsecond)
        {
            _ticks = CheckTimeSpanBound(new TimeSpan(0, hour, mins, second, millsecond));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        public Time(long ticks)
        {
            _ticks = CheckTimeSpanBound(new TimeSpan(ticks));
        }

        /// <summary>
        ///Gets the number of ticks that represents the value of the current <see cref="System.TimeSpan"/>
        /// </summary>
        public long Ticks
        {
            get { return _ticks.Ticks; }
        }

        /// <summary>
        ///Gets the hours component of time interval represented by the current <see cref="System.TimeSpan"/>
        /// </summary>
        public int Hours
        {
            get { return _ticks.Hours; }
        }

        /// <summary>
        /// Gets the minutes component of time interval represented by the current <see cref="System.TimeSpan"/>
        /// </summary>
        public int Minutes
        {
            get { return _ticks.Minutes; }
        }

        /// <summary>
        ///Gets the Secnonds component of time interval represented by the current <see cref="System.TimeSpan"/>
        /// </summary>
        public int Seconds
        {
            get { return _ticks.Seconds; }
        }

        /// <summary>
        ///Gets the milliseconds component of time interval represented by the current <see cref="System.TimeSpan"/>
        /// </summary>
        public int Millseconds
        {
            get { return _ticks.Milliseconds; }
        }

        /// <summary>
        /// Return a new time instance after add special number of hours to this instance
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public Time AddHours(int hour)
        {
            var result = new Time(_ticks.Add(new TimeSpan(hour, 0, 0)).Ticks);
            return CheckBound(result._ticks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        public Time AddMinutes(int min)
        {
            var result = new Time(_ticks.Add(new TimeSpan(0, min, 0)).Ticks);
            return CheckBound(result._ticks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public Time AddSeconds(int second)
        {
            var result = new Time(_ticks.Add(new TimeSpan(0, 0, second)).Ticks);
            return CheckBound(result._ticks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millsecond"></param>
        /// <returns></returns>
        public Time AddMillseconds(int millsecond)
        {
            var result = new Time(_ticks.Add(new TimeSpan(0, 0, 0, millsecond)).Ticks);
            return CheckBound(result._ticks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}", Hours, Minutes, Seconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            var time = new DateTime(_ticks.Ticks);
            return time.ToString(format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static TimeSpan operator -(Time a, Time b)
        {
            return a._ticks - b._ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static DateTime operator -(DateTime dt, Time t1)
        {
            long tick = dt.Ticks - t1._ticks.Ticks;
            return new DateTime(tick);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static TimeSpan operator +(Time a, Time b)
        {
            return a._ticks + b._ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static DateTime operator +(DateTime dt, Time t1)
        {
            long tick = dt.Ticks + t1._ticks.Ticks;
            return new DateTime(tick);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime operator +(Time t1, DateTime dt)
        {
            return dt + t1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Time a, Time b)
        {
            return a._ticks == b._ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Time a, Time b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >=(Time a, Time b)
        {
            return a._ticks >= b._ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <(Time a, Time b)
        {
            return a._ticks < b._ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return _ticks.GetHashCode()*3;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool result = base.Equals(obj);
            if (result)
                return true;
            if (obj.GetType() != GetType())
                return true;

            return ((Time) obj) == this;
        }
    }
}
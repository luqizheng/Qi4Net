using System;
using System.Collections.Generic;
using System.Threading;

namespace Qi
{
    /// <summary>
    /// Sameple cache
    /// </summary>
    public class QCache : IDisposable
    {
        private readonly Dictionary<int, TimeoutItem> _objectPools = new Dictionary<int, TimeoutItem>();
        private readonly SortedList<DateTime, int> _timeoutPools = new SortedList<DateTime, int>();

        #region IDisposable Members

        public void Dispose()
        {
            lock (_timeoutPools)
            {
                _timeoutPools.Clear();
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="milliSeconds"></param>
        /// <returns>key of obj </returns>
        /// <remarks></remarks>
        public int Add(object obj, int milliSeconds)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            DateTime timeout = DateTime.Now.AddMilliseconds(milliSeconds);
            while (_timeoutPools.ContainsKey(timeout))
            {
                timeout = timeout.AddMilliseconds(1);
            }

            lock (_timeoutPools)
            {
                _timeoutPools.Add(timeout, obj.GetHashCode());
                _objectPools.Add(obj.GetHashCode(), new TimeoutItem { MilliSeconds = milliSeconds, Target = obj });
            }
            if (_timeoutPools.Count == 1)
            {
                StartPooling();
            }
            return obj.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hascode"></param>
        /// <returns></returns>
        public object GetObj(int hascode)
        {
            if (!_objectPools.ContainsKey(hascode))
                throw new NotFoundCacheObjectException(hascode);

            TimeoutItem item = _objectPools[hascode];
            ThreadPool.QueueUserWorkItem(s =>
                                             {
                                                 //更新一下超时的时间。
                                                 var indexOfTimePool = _timeoutPools.IndexOfValue(hascode);
                                                 lock (_timeoutPools)
                                                 {
                                                     _timeoutPools.RemoveAt(indexOfTimePool);
                                                     var time = DateTime.Now.AddMilliseconds(item.MilliSeconds);
                                                     while (_timeoutPools.ContainsKey(time))
                                                     {
                                                         time = time.AddMilliseconds(1);
                                                     }
                                                     _timeoutPools.Add(time, item.Target.GetHashCode());
                                                     
                                                 }
                                             });
            return item.Target;
        }


        private void StartPooling()
        {
            ThreadPool.QueueUserWorkItem(state =>
                                             {
                                                 while (_timeoutPools.Count != 0)
                                                 {
                                                     int index = FindIndex(DateTime.Now);
                                                     if (index != -1)
                                                     {
                                                         lock (_timeoutPools)
                                                         {
                                                             lock (_objectPools)
                                                             {
                                                                 for (int i = index; i >= 0; i--)
                                                                 {
                                                                     int objHasCode = _timeoutPools.Values[i];
                                                                     _objectPools.Remove(objHasCode);
                                                                     _timeoutPools.RemoveAt(i);
                                                                 }
                                                             }
                                                         }
                                                         Thread.Sleep(100);
                                                     }
                                                 }
                                             });
        }

        private int FindIndex(DateTime lessThanDateTime)
        {
            int start = 0;
            int end = _timeoutPools.Count - 1;
            int middle = 0;
            while (end >= start)
            {
                middle = (end + start) / 2;
                DateTime val = _timeoutPools.Keys[middle].Date;
                if (val == lessThanDateTime)
                {
                    return _timeoutPools.IndexOfKey(lessThanDateTime);
                }
                if (val < lessThanDateTime)
                {
                    start = middle + 1;
                }
                else
                {
                    end = middle - 1;
                }
            }
            return end;
        }

        #region Nested type: TimeoutItem

        public class TimeoutItem
        {
            public int MilliSeconds;
            public object Target;
        }

        #endregion
    }
}
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
        bool _slideTime;
        Thread _thread;
        public QCache(bool slideTime)
        {
            _slideTime = slideTime;
        }
        public QCache()
            : this(false)
        {
        }
        #region IDisposable Members

        public void Dispose()
        {
            _thread.Abort();
            lock (_timeoutPools)
            {
                lock (_objectPools)
                {
                    _objectPools.Clear();
                    _timeoutPools.Clear();
                }
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

            if (_objectPools.ContainsKey(obj.GetHashCode()))
            {
                Console.WriteLine("constans obj,and refrsh");
                if (_slideTime)
                {
                    //refresh time;
                    var result = GetObj(obj.GetHashCode());
                }
                return obj.GetHashCode();
            }

            var now = DateTime.Now;
            DateTime timeout = now.AddMilliseconds(milliSeconds);
            while (_timeoutPools.ContainsKey(timeout))
            {
                timeout = timeout.AddMilliseconds(1);
            }

            lock (_timeoutPools)
            {
                lock (_objectPools)
                {
                    if (!_objectPools.ContainsKey(obj.GetHashCode()))
                    {
                        _timeoutPools.Add(timeout, obj.GetHashCode());
                        _objectPools.Add(obj.GetHashCode(), new TimeoutItem { MilliSeconds = milliSeconds, Target = obj });
                    }
                }
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
                return null;

            TimeoutItem item = _objectPools[hascode];
            if (_slideTime)
            {
                ThreadPool.QueueUserWorkItem(s =>
                                                 {

                                                     var objHasCode = (int)s;
                                                     //更新一下超时的时间。
                                                     var indexOfTimePool = _timeoutPools.IndexOfValue(objHasCode);
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

                                                 }, hascode);
            }
            return item.Target;
        }


        private void StartPooling()
        {
            if (_thread != null)
            {
                _thread.Abort();
            }

            _thread = new Thread(new ParameterizedThreadStart(Pooling));
            _thread.IsBackground = true;
            _thread.Start();

        }
        private void Pooling(object state)
        {
            try
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
                    }
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }
        private int FindIndex(DateTime lessThanDateTime)
        {
            int start = 0;
            int end = _timeoutPools.Count - 1;
            int middle = 0;
            while (end >= start)
            {
                middle = (end + start) / 2;
                DateTime val = _timeoutPools.Keys[middle];
                if (val == lessThanDateTime)
                {
                    return middle;
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
            if (end != -1 && _timeoutPools.Keys[end] > lessThanDateTime)
            {
                return end - 1;
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
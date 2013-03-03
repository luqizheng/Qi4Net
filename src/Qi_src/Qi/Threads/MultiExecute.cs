using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qi.Threads
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MultiExecute
    {
        private readonly int _threadCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadCount">启用多少个Thread去执行,最大9999,最少1</param>
        /// <exception cref="ArgumentOutOfRangeException">当<see cref="threadCount"/>不在1～9999范围之内</exception>
        public MultiExecute(int threadCount)
        {
            if (threadCount < 0 || threadCount > 9999)
                throw new ArgumentOutOfRangeException("threadCount", "Should be between 1~9999");
            _threadCount = threadCount;
            
            var a = Partitioner.Create(0, 100);
        }

        #region exeute handler with return Value

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="data"></param>
        /// <param name="executeHandler"></param>
        /// <param name="threadComplete"></param>
        /// <param name="callback"></param>
        public void Execute<T, TReturn>(T[] data, Func<T[], TReturn> executeHandler, VoidFunc<TReturn> threadComplete,
                                        VoidFunc callback)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length == 0)
                return;
            var handler = new VoidFunc<T[], Func<T[], TReturn>, VoidFunc<TReturn>>(ActualExecute);
            handler.BeginInvoke(data, executeHandler, threadComplete, a =>
                                                                          {
                                                                              if (callback != null)
                                                                                  callback();
                                                                          }, null);
        }

        #endregion

        #region without handler

        public void Execute<T>(T[] data, VoidFunc<T[]> executeFunc)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length == 0)
                return;
            var handler = new VoidFunc<T[], VoidFunc<T[]>, VoidFunc>(ActualExecute);
            handler.BeginInvoke(data, executeFunc, null, a => { }, null);
        }

        public void Execute<T>(T[] data, VoidFunc<T[]> executeFunc, VoidFunc callback)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length == 0)
                return;
            var handler = new VoidFunc<T[], VoidFunc<T[]>, VoidFunc>(ActualExecute);
            handler.BeginInvoke(data, executeFunc, null, a =>
                                                             {
                                                                 if (callback != null)
                                                                     callback();
                                                             }, null);
        }

        public void Execute<T>(T[] data, VoidFunc<T[]> executeFunc, VoidFunc threadComplete, VoidFunc callback)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (data.Length == 0) return;

            if (executeFunc == null) throw new ArgumentNullException("executeFunc");
            if (threadComplete == null) throw new ArgumentNullException("threadComplete");

            var handler = new VoidFunc<T[], VoidFunc<T[]>, VoidFunc>(ActualExecute);
            handler.BeginInvoke(data, executeFunc, threadComplete, a =>
                                                                       {
                                                                           if (callback != null)
                                                                               callback();
                                                                       }, null);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="executeHandler"> </param>
        /// <param name="threadComplete"> </param>
        private void ActualExecute<T>(T[] datas, VoidFunc<T[]> executeHandler, VoidFunc threadComplete)
        {
            Thread[] threads;
            var globalCheck = new ManualResetEvent(false);
            T[][] splitData = AssignThread(datas, out threads, state =>
                                                                   {
                                                                       executeHandler((T[]) state);
                                                                       if (threadComplete != null)
                                                                       {
                                                                           threadComplete();
                                                                       }
                                                                       globalCheck.Set();
                                                                   });

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start(splitData[i]);
            }

            WaitAllDone(threads, globalCheck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="executeHandler"></param>
        /// <param name="threadComplete">可以为null</param>
        private void ActualExecute<T, TReturn>(T[] datas, Func<T[], TReturn> executeHandler,
                                               VoidFunc<TReturn> threadComplete)
        {
            Thread[] threads;
            var globalCheck = new ManualResetEvent(false);
            T[][] splitData = AssignThread(datas, out threads, state =>
                                                                   {
                                                                       TReturn retrunValue = executeHandler((T[]) state);
                                                                       if (threadComplete != null)
                                                                       {
                                                                           threadComplete(retrunValue);
                                                                       }
                                                                       globalCheck.Set();
                                                                   });

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start(splitData[i]);
            }

            WaitAllDone(threads, globalCheck);
        }

        private static void WaitAllDone(Thread[] threads, ManualResetEvent globalCheck)
        {
            while (true)
            {
                globalCheck.WaitOne();
                if (AllDone(threads))
                {
                    break;
                }
            }
        }


        private T[][] AssignThread<T>(T[] datas, out Thread[] threads, ParameterizedThreadStart completeCallback)
        {
            T[][] result = datas.ToArray().DivEqual(_threadCount);
            threads = new Thread[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                threads[i] = new Thread(completeCallback) {IsBackground = true};
            }
            datas = null;
            return result;
        }

        private static bool AllDone(IEnumerable<Thread> threads)
        {
            bool allfinish = false;
            foreach (Thread t in threads)
            {
                if (t.IsAlive)
                {
                    break;
                }
                allfinish = true;
            }
            return allfinish;
        }
    }
}
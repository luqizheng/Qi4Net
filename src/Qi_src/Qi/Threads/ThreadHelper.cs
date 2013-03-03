using System;
using System.Threading;

namespace Qi.Threads
{
    public static class ThreadHelper
    {
        /// <summary>
        /// 以同步的方式自动分配数据到多个线程中区
        /// </summary
        /// <typeparam name="T"></typeparam>
        /// <param name="threadCount"></param>
        /// <param name="data"></param>
        /// <param name="executeFunction"></param>
        public static void AvgExecute<T>(this T[] data, int threadCount,
                                         VoidFunc<T[]> executeFunction)
        {
            var thread = new MultiExecute(threadCount);
            thread.Execute(data, executeFunction);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="threadCount"></param>
        /// <param name="executeFunc"></param>
        /// <param name="threadCompleteHandler"></param>
        /// <param name="callback"></param>
        public static void AvgExecute<T>(this T[] data, int threadCount, VoidFunc<T[]> executeFunc, VoidFunc threadCompleteHandler, VoidFunc callback)
        {
            var thread = new MultiExecute(threadCount);
            thread.Execute(data, executeFunc, threadCompleteHandler, callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="threadCount"></param>
        /// <param name="executeFunc"></param>
        /// <param name="callback"></param>
        public static void AvgExecute<T>(this T[] data, int threadCount, VoidFunc<T[]> executeFunc, VoidFunc callback)
        {
            var thread = new MultiExecute(threadCount);
            thread.Execute(data, executeFunc, callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="data"></param>
        /// <param name="threadCount"></param>
        /// <param name="executeFunc"></param>
        /// <param name="threadCompleteHandler"></param>
        /// <param name="callback"></param>
        public static void AvgExecute<T, TReturn>(this T[] data, int threadCount, Func<T[], TReturn> executeFunc, VoidFunc<TReturn> threadCompleteHandler, VoidFunc callback)
        {
            var thread = new MultiExecute(threadCount);
            thread.Execute(data, executeFunc, threadCompleteHandler, callback);
        }

        


        /// <summary>
        /// execute and callback
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="callback"></param>
        public static void Execute(VoidFunc execute, VoidFunc callback)
        {
            ThreadPool.QueueUserWorkItem(s =>
                                             {
                                                 execute();
                                                 callback();
                                             });
        }
    }
}
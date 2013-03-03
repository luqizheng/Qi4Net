using System;
using System.Collections;
using System.Collections.Generic;

namespace Qi.Web.JsonContainers
{
    internal abstract class Accesser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        protected Accesser(IEnumerable target)
        {
            if (target == null) throw new ArgumentNullException("target");
            Target = target;
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract int Count { get; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable Target { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="startIndex"></param>
        public abstract void Set(IEnumerable objects, int startIndex);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Accesser Create(object target)
        {
            var array = target as Array;
            if (array != null)
                return new ArrayAccesser(array);
            return new ListAccesser((IList) target);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="convert"></param>
        /// <returns></returns>
        public T[] ToArray<T>(Func<object, T> convert)
        {
            var result = new T[Count];
            bool isJsonContainer = typeof (T) == typeof (JsonContainer);
            int i = 0;
            foreach (object item in Target)
            {
                if (isJsonContainer)
                {
                    result.SetValue(new JsonContainer((Dictionary<string, object>) item), i);
                }
                else
                {
                    result.SetValue(convert(item), i);
                }
                i++;
            }
            return result;
        }
    }
}
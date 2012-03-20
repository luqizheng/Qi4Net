using System;
using System.Collections;
using System.Collections.Generic;

namespace Qi.Web.JsonContainers
{
    internal abstract class Accesser
    {
        protected Accesser(IEnumerable target)
        {
            Target = target;
        }
        public abstract void Set(IEnumerable objects, int startIndex);
        public abstract int Count { get; }
        public IEnumerable Target { get; private set; }
        public static Accesser Create(object target)
        {
            var array = target as Array;
            if (array != null)
                return new ArrayAccesser(array);
            return new ListAccesser((IList)target);
        }

        public T[] ToArray<T>(Func<object, T> convert)
        {
            var result = new T[this.Count];
            bool isJsonContainer = typeof(T) == typeof(JsonContainer);
            int i = 0;
            foreach (object item in Target)
            {
                if (isJsonContainer)
                {
                    result.SetValue(new JsonContainer((Dictionary<string, object>)item), i);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi
{
    public class LazyInitialization
    {
        private readonly Dictionary<int, object> _inits = new Dictionary<int, object>();
        /// <summary>
        /// Init once and return the new one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initMethod"></param>
        /// <returns></returns>
        public T Once<T>(Func<T> initMethod)
        {
            var key = initMethod.GetHashCode();
            if (!_inits.ContainsKey(key))
            {
                _inits.Add(key, initMethod());
            }
            return (T)_inits[key];
        }
        
    }
}

using System;
using System.Collections.Generic;

namespace Qi
{
    public class ObjectPools
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
            int key;
            return Once(initMethod, out key);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initMethod"></param>
        /// <param name="key">use key can found it in cache.</param>
        /// <returns></returns>
        public T Once<T>(Func<T> initMethod, out int key)
        {
            key = initMethod.GetHashCode();
            if (!_inits.ContainsKey(key))
            {
                _inits.Add(key, initMethod());
            }
            return (T)_inits[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initMethod"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T WeakReference<T>(Func<T> initMethod, out int key)
        {
            key = initMethod.GetHashCode();

            if (!_inits.ContainsKey(key))
            {
                _inits.Add(key, new WeakReference(initMethod()));
            }
            var weObject = (WeakReference)_inits[key];
            if (weObject.IsAlive)
            {
                return (T)weObject.Target;
            }
            _inits[key] = new WeakReference(initMethod());
            return (T)((WeakReference)_inits[key]).Target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initMethod"></param>
        /// <returns></returns>
        public T WeakReference<T>(Func<T> initMethod)
        {
            int key;
            return WeakReference(initMethod, out key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetObject<T>(int key)
        {
            if (!_inits.ContainsKey(key))
                throw new NotFoundCacheObjectException(key);

            object result = _inits[key];
            var timeoutItem = result as WeakReference;
            if (timeoutItem != null)
            {
                if (!timeoutItem.IsAlive)
                {
                    throw new NotFoundCacheObjectException(key);
                }
                return (T)timeoutItem.Target;
            }
            return (T)result;
        }
    }
}
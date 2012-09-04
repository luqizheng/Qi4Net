using System.Collections;
using Qi.Web;

namespace Qi.SharePools.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpStore : IStore
    {
        private IDictionary _dictionary;
        /// <summary>
        /// 
        /// </summary>
        private IDictionary Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    lock (this)
                    {
                        object httpContext = ReflectiveHttpContext.HttpContextCurrentGetter();
                        _dictionary = ReflectiveHttpContext.HttpContextItemsGetter(httpContext);
                    }
                }
                return _dictionary;
            }
        }

        #region IStore Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetData(string key, object data)
        {
            if (Dictionary.Contains(key))
            {
                Dictionary[key] = data;
            }
            else
            {
                Dictionary.Add(key, data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            if (!Dictionary.Contains(key))
            {
                return null;
            }
            return Dictionary[key];
        }

        #endregion
    }
}
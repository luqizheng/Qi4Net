using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Qi.Web;

namespace Qi.SharePools
{
    public class HttpStore : IStore
    {
        private IDictionary _dictionary;

        private IDictionary Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    lock (this)
                    {
                        var httpContext = ReflectiveHttpContext.HttpContextCurrentGetter();
                        _dictionary = ReflectiveHttpContext.HttpContextItemsGetter(httpContext);
                    }
                }
                return _dictionary;
            }
        }

        #region IStore Members

        public void SetData(string key, object data)
        {
            if (!Dictionary.Contains(key))
            {
                Dictionary[key] = data;
            }
            else
            {
                Dictionary.Add(key, data);
            }
        }

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
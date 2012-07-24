using System.Collections.Generic;
using Qi.SharePools;

namespace Qi.SharePools
{
    public class ThreadStaticStore : IStore
    {
        private readonly IDictionary<string, object> _pools = new Dictionary<string, object>();

        #region IStore Members

        public void SetData(string key, object data)
        {
            if (_pools.ContainsKey(key))
            {
                _pools.Add(key, data);
            }
            else
            {
                _pools[key] = data;
            }
        }

        public object GetData(string key)
        {
            if (!_pools.ContainsKey(key))
            {
                return null;
            }
            return _pools[key];
        }

      

        #endregion
    }
}
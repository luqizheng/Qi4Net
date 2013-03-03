using System.Collections.Generic;

namespace Qi.SharePools.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public class ThreadStaticStore : IStore
    {
        private readonly IDictionary<string, object> _pools = new Dictionary<string, object>();

        #region IStore Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetData(string key, object data)
        {
            if (!_pools.ContainsKey(key))
            {
                _pools.Add(key, data);
            }
            else
            {
                _pools[key] = data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
using System.Runtime.Remoting.Messaging;

namespace Qi.SharePools.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public class CallStore : IStore
    {
        #region IStore Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetData(string key, object data)
        {
            CallContext.SetData(key, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            return CallContext.GetData(key);
        }

        #endregion
    }
}
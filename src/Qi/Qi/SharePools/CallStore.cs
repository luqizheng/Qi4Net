using System.Runtime.Remoting.Messaging;
using Qi.SharePools;

namespace Qi.SharePools
{
    public class CallStore : IStore
    {
        #region IStore Members

        public void SetData(string key, object data)
        {
            CallContext.SetData(key, data);
        }

        public object GetData(string key)
        {
            return CallContext.GetData(key);
        }

        #endregion
    }
}
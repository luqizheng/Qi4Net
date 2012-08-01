using Qi.SharePools.Stores;

namespace Qi.SharePools
{
    /// <summary>
    /// 
    /// </summary>
    public class SharePool
    {
        /// <summary>
        /// 
        /// </summary>
        public static IStore ThreadStaticStore
        {
            get { return ThreadStatcBuilder.Instance; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static IStore HttpStore
        {
            get { return HttpStoreBuilder.Instance; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static IStore CallContextStore
        {
            get { return CallStoreBuilder.Instance; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static IStore DefaultStore
        {
            get
            {
                if (ApplicationHelper.IsWeb)
                {
                    return HttpStore;
                }
                return CallContextStore;
            }
        }

        #region Nested type: CallStoreBuilder
        /// <summary>
        /// 
        /// </summary>
        private class CallStoreBuilder
        {
            public static readonly IStore Instance = new CallStore();
        }

        #endregion

        #region Nested type: HttpStoreBuilder
        /// <summary>
        /// 
        /// </summary>
        private class HttpStoreBuilder
        {
            public static readonly IStore Instance = new HttpStore();
        }

        #endregion

        #region Nested type: ThreadStatcBuilder
        /// <summary>
        /// 
        /// </summary>
        private class ThreadStatcBuilder
        {
            public static readonly IStore Instance = new ThreadStaticStore();
        }

        #endregion
    }
}
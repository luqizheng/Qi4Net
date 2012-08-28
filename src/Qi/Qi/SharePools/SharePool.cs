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
            get { return ThreadStatcBuilder.Instance.Store; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IStore HttpStore
        {
            get { return HttpStoreBuilder.Instance.Store; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IStore CallContextStore
        {
            get { return CallStoreBuilder.Instance.Store; }
        }

        /// <summary>
        /// Get the Default Store, if in web context, return HttpStore, or CallContext
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
        private class CallStoreBuilder : IStoreFactory
        {
            public static readonly IStoreFactory Instance = new CallStoreBuilder();

            private CallStoreBuilder()
            {
                Store = new CallStore();
            }

            #region IStoreFactory Members

            public IStore Store { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: HttpStoreBuilder

        /// <summary>
        /// 
        /// </summary>
        private class HttpStoreBuilder : IStoreFactory
        {
            public static readonly IStoreFactory Instance = new HttpStoreBuilder();

            private HttpStoreBuilder()
            {
                Store = new HttpStore();
            }

            #region IStoreFactory Members

            public IStore Store { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: ThreadStatcBuilder

        /// <summary>
        /// 
        /// </summary>
        private class ThreadStatcBuilder : IStoreFactory
        {
            public static readonly IStoreFactory Instance = new ThreadStatcBuilder();

            private ThreadStatcBuilder()
            {
                Store = new ThreadStaticStore();
            }

            #region IStoreFactory Members

            public IStore Store { get; set; }

            #endregion
        }

        #endregion
    }
}
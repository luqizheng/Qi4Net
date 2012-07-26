namespace Qi.SharePools
{
    public class SharePool
    {
        public static IStore ThreadStaticStore
        {
            get { return ThreadStatcBuilder.Instance; }
        }

        public static IStore HttpStore
        {
            get { return HttpStoreBuilder.Instance; }
        }

        public static IStore CallContextStore
        {
            get { return CallStoreBuilder.Instance; }
        }

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

        private class CallStoreBuilder
        {
            public static readonly IStore Instance = new CallStore();
        }

        #endregion

        #region Nested type: HttpStoreBuilder

        private class HttpStoreBuilder
        {
            public static readonly IStore Instance = new HttpStore();
        }

        #endregion

        #region Nested type: ThreadStatcBuilder

        private class ThreadStatcBuilder
        {
            public static readonly IStore Instance = new ThreadStaticStore();
        }

        #endregion
    }
}
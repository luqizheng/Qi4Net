using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using Qi.SharePools;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public class SessionManager
    {
        internal static readonly SortedDictionary<string, SessionWrapper> Factories =
            new SortedDictionary<string, SessionWrapper>();

        private static readonly Dictionary<string, Func<Configuration>> LazyLoadConfig =
            new Dictionary<string, Func<Configuration>>();

        private static string _defaultSessionFactoryKey = String.Empty;

        /// <summary>
        /// </summary>
        public static readonly SessionManager Instance = new SessionManager();

        /// <summary>
        /// </summary>
        public string CurrentSessionFactoryName
        {
            get { return (string) SharePool.DefaultStore.GetData("session.key.factory."); }
            set { SharePool.DefaultStore.SetData("session.key.factory.", value); }
        }

        /// <summary>
        ///     default sesison factory key in the configruation file.
        /// </summary>
        public static string DefaultSessionFactoryKey
        {
            get
            {
                if (_defaultSessionFactoryKey == String.Empty)
                {
                    if (LazyLoadConfig.Count == 0)
                        throw new SessionManagerException("SessionManager do not include any SessionFactory");
                    _defaultSessionFactoryKey = LazyLoadConfig.Keys.First();
                }
                return _defaultSessionFactoryKey;
            }
        }

        /// <summary>
        /// </summary>
        public static string[] SessionFactoryNames
        {
            get { return LazyLoadConfig.Keys.ToArray(); }
        }

        /// <summary>
        ///     May be null, if you are not set the <see cref="CurrentSessionFactoryName" /> key
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SessionManagerException"></exception>
        public ISession GetCurrentSession()
        {
            if (CurrentSessionFactoryName == null)
            {
                CurrentSessionFactoryName = DefaultSessionFactoryKey;
            }
            return GetSessionWrapper(CurrentSessionFactoryName).CurrentSession;
        }

        /// <summary>
        ///     Get the default Session wrapper
        /// </summary>
        /// <returns></returns>
        public static SessionWrapper GetSessionWrapper()
        {
            return GetSessionWrapper(DefaultSessionFactoryKey);
        }

        /// <summary>
        ///     gets the session factory by session factory name defined in the configruation file.
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">sessionFactoryName is not exist in session manager</exception>
        public static SessionWrapper GetSessionWrapper(string sessionFactoryName)
        {
            if (!LazyLoadConfig.ContainsKey(sessionFactoryName))
            {
                throw new ArgumentOutOfRangeException("sessionFactoryName",
                                                      string.Format("can't find the {0} session factory.",
                                                                    sessionFactoryName));
            }
            if (!Factories.ContainsKey(sessionFactoryName))
            {
                Configuration config = LazyLoadConfig[sessionFactoryName].Invoke();
                var sessionwrapper = new SessionWrapper(config);
                lock (Factories)
                {
                    if (!Factories.ContainsKey(sessionFactoryName))
                    {
                        Factories.Add(sessionFactoryName, sessionwrapper);
                    }
                }
            }
            return Factories[sessionFactoryName];
        }

        /// <summary>
        /// </summary>
        /// <param name="sessionFacotryName"></param>
        /// <param name="initConfigLazy"></param>
        /// <exception cref="ArgumentNullException">sessionFacotryName or configruation is null</exception>
        /// <exception cref="SessionManagerException">SessionManager has this sessionFacotryName</exception>
        public static void Regist(string sessionFacotryName, Func<Configuration> initConfigLazy)
        {
            if (LazyLoadConfig.ContainsKey(sessionFacotryName) || Factories.ContainsKey(sessionFacotryName))
            {
                throw new SessionManagerException("sessionFacotryName", "Session Factory is regist, please try another.");
            }
            LazyLoadConfig.Add(sessionFacotryName, initConfigLazy);
        }
    }
}
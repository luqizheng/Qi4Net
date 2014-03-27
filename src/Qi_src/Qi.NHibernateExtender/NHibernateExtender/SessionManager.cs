using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;

namespace Qi.NHibernateExtender
{
    public class SessionFactoryProxy
    {
        public SessionFactoryProxy(Configuration configuration, string name)
        {
            Configuration = configuration;
            Name = name;
        }

        public Configuration Configuration { get; private set; }
        public string Name { get; private set; }

        public SessionWrapper Create()
        {
            return new SessionWrapper(Configuration);
        }
    }

    /// <summary>
    /// </summary>
    public class SessionManager
    {
        internal static readonly SortedDictionary<string, SessionFactoryProxy> Factories =
            new SortedDictionary<string, SessionFactoryProxy>();

        /// <summary>
        ///     config都是调用的时候再创建
        /// </summary>
        private static readonly Dictionary<string, Func<Configuration>> LazyLoadConfig =
            new Dictionary<string, Func<Configuration>>();

        private static string _defaultSessionFactoryKey = String.Empty;

        /// <summary>
        /// </summary>
        public static readonly SessionManager Instance = new SessionManager();

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
            if (string.IsNullOrEmpty(sessionFactoryName))
            {
                throw new ArgumentNullException("sessionFactoryName", "sessionFactoryName can not be null or empty");
            }

            if (!LazyLoadConfig.ContainsKey(sessionFactoryName))
            {
                throw new SessionManagerException(string.Format("can't find the {0} session factory.",
                    sessionFactoryName));
            }
            if (!Factories.ContainsKey(sessionFactoryName))
            {
                Configuration config = LazyLoadConfig[sessionFactoryName].Invoke();
                var factory = new SessionFactoryProxy(config, sessionFactoryName);
                lock (Factories)
                {
                    if (!Factories.ContainsKey(sessionFactoryName))
                    {
                        Factories.Add(sessionFactoryName, factory);
                    }
                }
            }
            return Factories[sessionFactoryName].Create();
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
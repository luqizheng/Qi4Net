using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Context;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public static class SessionManager
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
        /// </summary>
        /// <returns></returns>
        public static bool IsOpen()
        {
            return Factories.Keys.Any(IsOpen);
        }

        /// <summary>
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns></returns>
        public static bool IsOpen(string sessionFactoryName)
        {
            return CurrentSessionContext.HasBind(GetSessionWrapperFactory(sessionFactoryName).SessionFactory);
        }

        /// <summary>
        /// </summary>
        /// <param name="factroyName"></param>
        /// <returns></returns>
        public static SessionWrapper GetSessionWrapper(string factroyName)
        {
            return GetSessionWrapperFactory(factroyName).GetWrapper();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static SessionWrapper OpenNewSessionWrappeer()
        {
            return GetSessionWrapperFactory(DefaultSessionFactoryKey).CreateNewWrapper();
        }

        /// <summary>
        /// </summary>
        /// <param name="factroyName"></param>
        /// <returns></returns>
        public static SessionWrapper OpenNewSessionWrappeer(string factroyName)
        {
            return GetSessionWrapperFactory(factroyName).CreateNewWrapper();
        }


        /// <summary>
        ///     gets the session factory by session factory name defined in the configruation file.
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">sessionFactoryName is not exist in session manager</exception>
        public static SessionFactoryProxy GetSessionWrapperFactory(string sessionFactoryName)
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

        /// <summary>
        ///     关闭所有的Session
        /// </summary>
        /// <param name="submit"></param>
        public static void CloseAll(bool submit)
        {
            foreach (SessionFactoryProxy key in Factories.Values)
            {
                if (CurrentSessionContext.HasBind(key.SessionFactory))
                {
                    var session = CurrentSessionContext.Unbind(key.SessionFactory) as SessionProxy;
                    if (session != null)
                    {
                        session.CloseCascade();
                    }
                }
            }
        }
    }
}
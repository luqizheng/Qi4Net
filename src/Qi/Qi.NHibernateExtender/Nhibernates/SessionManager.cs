using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;

namespace Qi.Nhibernates
{
    public class SessionManager : IDisposable
    {
        private static readonly SortedDictionary<string, SessionManager> SessionManagers =
            new SortedDictionary<string, SessionManager>();


        private readonly NhConfig _configInfo;

        static SessionManager()
        {
            NhConfig[] nh = NhConfig.GetNHFileInfos();
            for (int i = 0; i < nh.Length; i++)
            {
                Add(nh[i], i == 0 || nh[i].SessionFactoryName == "default");
            }
        }

        private SessionManager(NhConfig nhConfig)
        {
            if (nhConfig == null)
                throw new ArgumentNullException("nhConfig");

            _configInfo = nhConfig;
        }

        public static string DefaultSessionFactoryKey { get; private set; }

        /// <summary>
        /// Gets session manager instance;
        /// </summary>
        public static SessionManager Instance
        {
            get { return SessionManagers[DefaultSessionFactoryKey]; }
        }


        /// <summary>
        /// Gets Session after use <see cref="IniSession"/>
        /// </summary>
        public ISession CurrentSession
        {
            get { return GetSessionFactory().GetCurrentSession(); }
        }

        /// <summary>
        /// 需要自己处理 session
        /// </summary>
        public ISession NewSession
        {
            get { return GetSessionFactory().OpenSession(); }
        }

        /// <summary>
        /// Sessionless,需要自行处理close 
        /// </summary>
        public IStatelessSession Sessionless
        {
            get { return GetSessionFactory().OpenStatelessSession(); }
        }

        public NhConfig Config
        {
            get { return _configInfo; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            CleanUp();
        }

        #endregion

        public static IEnumerable<SessionManager> GetAll()
        {
            return SessionManagers.Values;
        }

        public static SessionManager GetInstance(Type entityType)
        {
            foreach (string key in SessionManagers.Keys)
            {
                var sfi = (ISessionFactoryImplementor) SessionManagers[key].GetSessionFactory();
                if (sfi.GetEntityPersister(entityType.FullName) != null)
                    return GetInstance(key);
            }
            throw new ArgumentNullException("Can't find the SessionFactory include " + entityType.FullName);
        }

        public static SessionManager GetInstance(string sessionFactoryName)
        {
            if (!SessionManagers.ContainsKey(sessionFactoryName))
                throw new NhConfigurationException("Can't find the session-factory named " + sessionFactoryName);
            return SessionManagers[sessionFactoryName];
        }


        public ISessionFactory GetSessionFactory()
        {
            return _configInfo.SessionFactory;
        }

        public void CleanUp()
        {
            if (CurrentSessionContext.HasBind(GetSessionFactory()))
            {
                ISession currentSession = GetSessionFactory().GetCurrentSession();

                if (currentSession.Transaction != null && currentSession.Transaction.IsActive)
                {
                    currentSession.Transaction.Commit();
                }
                currentSession.Flush();
                currentSession.Close();
                CurrentSessionContext.Unbind(GetSessionFactory());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>初始化了Session 返回true，如果调用这个方法之前已经初始化，返回false</returns>
        public bool IniSession()
        {
            if (!CurrentSessionContext.HasBind(GetSessionFactory()))
            {
                ISession session = GetSessionFactory().OpenSession();
                CurrentSessionContext.Bind(session);
                return true;
            }
            return false;
        }

        public static void CleanAll()
        {
            foreach (SessionManager a in SessionManagers.Values)
            {
                a.CleanUp();
            }
        }

        public static void Add(NhConfig info, bool isDefault)
        {
            if (SessionManagers.ContainsKey(info.SessionFactoryName))
            {
                throw new NhConfigurationException("Can't add exist config file.");
            }

            SessionManagers.Add(info.SessionFactoryName, new SessionManager(info));
            if (isDefault)
            {
                DefaultSessionFactoryKey = info.SessionFactoryName;
            }
        }

        public static void Remove(NhConfig info)
        {
            if (info == null) throw new ArgumentNullException("info");
            SessionManagers.Remove(info.SessionFactoryName);
        }

        public static void SetDefault(string sessionFactoryKey)
        {
            if (!SessionManagers.ContainsKey(sessionFactoryKey))
                throw new ArgumentOutOfRangeException("sessionFactoryKey",
                                                      string.Format("can't find session key named {0} in the pools.",
                                                                    sessionFactoryKey));
            DefaultSessionFactoryKey = SessionManagers[sessionFactoryKey].Config.SessionFactoryName;
        }
    }
}
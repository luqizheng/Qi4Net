using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;
using NHibernate.Stat;

namespace Qi.Nhibernates
{
    public class SessionManager : IDisposable
    {
        private const string CurrentSessionFactoryName = "session.factory.name.qi.";
        private const string InitKeyName = "session.is.Initiated";

        private static readonly SortedDictionary<string, ISessionFactory> Factories =
            new SortedDictionary<string, ISessionFactory>();

        private static readonly SessionManager _instance = new SessionManager();

        /// <summary>
        /// default sesison factory key in the config file.
        /// </summary>
        private static readonly string DefaultSessionFactoryKey;


        static SessionManager()
        {
            //创建配置有的sessionFactory放入这里
            foreach (string sessionFactoryKey in NhConfigManager.SessionFactoryNames)
            {
                if (CurrentSessionFactoryKey == null || sessionFactoryKey == "default")
                    DefaultSessionFactoryKey = sessionFactoryKey;
                Factories.Add(sessionFactoryKey, NhConfigManager.GetNhConfig(sessionFactoryKey).BuildSessionFactory());
            }
        }

        /// <summary>
        /// 默认SessionManager
        /// </summary>
        private SessionManager()
        {
        }

        public bool IsInitiated
        {
            get
            {
                object obj = CallContext.GetData(InitKeyName);
                if (obj == null)
                    return false;
                return (bool) obj;
            }
            private set { CallContext.SetData(InitKeyName, value); }
        }

        public static string CurrentSessionFactoryKey
        {
            get { return (CallContext.GetData(CurrentSessionFactoryName) as string) ?? DefaultSessionFactoryKey; }
            set
            {
                if (!Factories.ContainsKey(value))
                    throw new ArgumentOutOfRangeException("value",
                                                          string.Format(
                                                              "Can't find the session factory name {0} in the setting.",
                                                              value));
                CallContext.SetData(CurrentSessionFactoryName, value);
            }
        }

        /// <summary>
        /// Gets session manager instance;
        /// </summary>
        public static SessionManager Instance
        {
            get { return _instance; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            //it only happend in the application shut down
            foreach (ISessionFactory a in Factories.Values)
            {
                a.Close();
                a.Dispose();
            }
            Factories.Clear();
        }

        #endregion

        public bool InitSession()
        {
            if (!IsInitiated)
            {
                IsInitiated = true;
                return true;
            }
            return false;
        }

        public static void Add(string sessionName, ISessionFactory config)
        {
            if (Factories.ContainsKey(sessionName))
                throw new ArgumentNullException(string.Format("session factory name {0} has defined.", sessionName));

            Factories.Add(sessionName, config);
        }

        public static void Remove(string sessionName)
        {
            if (!Factories.ContainsKey(sessionName))
                throw new ArgumentNullException(string.Format("session factory name {0} is not defined.", sessionName));
            Factories.Remove(sessionName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ISession GetCurrentSession()
        {
            ISessionFactory sf = GetSessionFactory(CurrentSessionFactoryKey);

            if (!CurrentSessionContext.HasBind(sf))
            {
                CurrentSessionContext.Bind(sf.OpenSession());
            }
            return sf.GetCurrentSession();
        }

        public ISession GetCurrentSession(Type entityType)
        {
            ISessionFactory sf = GetSessionFactory(entityType);
            if (!CurrentSessionContext.HasBind(sf))
            {
                CurrentSessionContext.Bind(sf.OpenSession());
            }
            return sf.GetCurrentSession();
        }

        public ISession GetCurrentSession(string sfName)
        {
            ISessionFactory sf = GetSessionFactory(sfName);

            if (!CurrentSessionContext.HasBind(sf))
            {
                IStatistics a = sf.Statistics;
                CurrentSessionContext.Bind(sf.OpenSession());
            }
            return sf.GetCurrentSession();
        }

        /// <summary>
        /// gets the session factory by session factory name defined in the config file.
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="sessionFactoryName"/> is not exist in session manager</exception>
        public ISessionFactory GetSessionFactory(string sessionFactoryName)
        {
            if (!Factories.ContainsKey(sessionFactoryName))
                throw new ArgumentOutOfRangeException("sessionFactoryName",
                                                      string.Format("can't find the{0}session factory.",
                                                                    sessionFactoryName));
            return Factories[sessionFactoryName];
        }

        public ISessionFactory GetSessionFactory(Type entityType)
        {
            foreach (ISessionFactoryImplementor sf in Factories.Values)
            {
                if (sf.GetEntityPersister(entityType.FullName) != null)
                    return sf;
            }
            throw new SessionException(string.Format("can't find the mapping entity with {0}", entityType));
        }

        public void ClearUp(params ISessionFactory[] factories)
        {
            foreach (ISessionFactory sf in factories)
            {
                if (CurrentSessionContext.HasBind(sf))
                {
                    sf.GetCurrentSession().Flush();
                    ISession session = CurrentSessionContext.Unbind(sf);
                    session.Close();
                }
            }
        }

        public void CleanUp()
        {
            ClearUp(Factories.Values.ToArray());
        }
    }
}
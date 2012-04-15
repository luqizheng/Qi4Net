using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using NHibernate;
using NHibernate.Context;
using NHibernate.Stat;

namespace Qi.Nhibernates
{
    public class SessionManager : IDisposable
    {
        private const string CurrentSessionFactoryName = "session.factory.name.qi.";
        private const string InitKeyName = "session.is.Initiated";

        private static readonly SortedDictionary<string, ISessionFactory> Factories =
            new SortedDictionary<string, ISessionFactory>();

        // ReSharper disable InconsistentNaming
        private static readonly SessionManager _instance = new SessionManager();
        // ReSharper restore InconsistentNaming

        private static string _defaultSessionFactoryKey = String.Empty;

        /// <summary>
        /// 默认SessionManager
        /// </summary>
        private SessionManager()
        {
        }

        /// <summary>
        /// default sesison factory key in the sessionFactory file.
        /// </summary>
        private static string DefaultSessionFactoryKey
        {
            get
            {
                if (_defaultSessionFactoryKey == String.Empty)
                {
                    lock (_defaultSessionFactoryKey)
                    {
                        if (_defaultSessionFactoryKey == String.Empty)
                        {
                            //Found the default Key
                            string[] names = NhConfigManager.SessionFactoryNames;
                            _defaultSessionFactoryKey = names[0];
                            for (int i = 1; i < names.Length; i++)
                            {
                                if (names[i] == "default")
                                {
                                    _defaultSessionFactoryKey = names[i];
                                }
                            }
                        }
                    }
                }
                return _defaultSessionFactoryKey;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInitiated
        {
            get
            {
                object obj = CallContext.GetData(InitKeyName);
                if (obj == null)
                    return false;
                return (bool)obj;
            }
            private set { CallContext.SetData(InitKeyName, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string CurrentSessionFactoryKey
        {
            get { return (CallContext.GetData(CurrentSessionFactoryName) as string) ?? DefaultSessionFactoryKey; }
            set { CallContext.SetData(CurrentSessionFactoryName, value); }
        }

        /// <summary>
        /// Gets session manager instance;
        /// </summary>
        public static SessionManager Instance
        {
            get { return _instance; }
        }

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool InitSession()
        {
            if (!IsInitiated)
            {
                IsInitiated = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// add session facotry in SessionManager.
        /// </summary>
        /// <param name="sessionFacotryName"></param>
        /// <param name="sessionFactory"></param>
        /// <exception cref="ArgumentNullException">sessionFacotryName or sessionFactory is null</exception>
        /// <exception cref="SessionManagerException">SessionManager has this sessionFacotryName</exception>
        public static void Add(string sessionFacotryName, ISessionFactory sessionFactory)
        {
            if (String.IsNullOrEmpty(sessionFacotryName)) throw new ArgumentNullException("sessionFacotryName");
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");
            if (Factories.ContainsKey(sessionFacotryName))
                throw new SessionManagerException(string.Format("session factory name {0} has defined.", sessionFacotryName));

            Factories.Add(sessionFacotryName, sessionFactory);
        }

        public static void Remove(string sessionFactoryName)
        {
            if (String.IsNullOrEmpty(sessionFactoryName)) throw new ArgumentNullException("sessionFactoryName");
            if (!Factories.ContainsKey(sessionFactoryName))
                throw new ArgumentNullException(string.Format("session factory name {0} is not defined.", sessionFactoryName));
            Factories.Remove(sessionFactoryName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ISession GetCurrentSession()
        {
            return GetCurrentSession(CurrentSessionFactoryKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        /// <exception cref="SessionManagerException">Init session first</exception>
        public ISession GetCurrentSession(Type entityType)
        {
            if (!IsInitiated)
            {
                throw new SessionManagerException("Please use InitSession() before use GetCurrentSession()");
            }
            ISessionFactory sf = GetSessionFactory(entityType);
            if (!CurrentSessionContext.HasBind(sf))
            {
                CurrentSessionContext.Bind(sf.OpenSession());
            }
            return sf.GetCurrentSession();
        }

        public ISession GetCurrentSession(string sfName)
        {
            if (!IsInitiated)
            {
                throw new SessionManagerException("Please use InitSession() before use GetCurrentSession()");
            }
            ISessionFactory sf = GetSessionFactory(sfName);

            if (!CurrentSessionContext.HasBind(sf))
            {
                IStatistics a = sf.Statistics;
                CurrentSessionContext.Bind(sf.OpenSession());
            }
            return sf.GetCurrentSession();
        }

        /// <summary>
        /// gets the session factory by session factory name defined in the sessionFactory file.
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="sessionFactoryName"/> is not exist in session manager</exception>
        public ISessionFactory GetSessionFactory(string sessionFactoryName)
        {
            // In order to lazy init the session.
            INhConfig nhFileConfig;
            if (!Factories.ContainsKey(sessionFactoryName))
            {
                nhFileConfig = NhConfigManager.GetNhConfig(sessionFactoryName);
                if (nhFileConfig == null)
                {
                    throw new ArgumentOutOfRangeException("sessionFactoryName",
                                                          string.Format("can't find the{0}session factory.",
                                                                        sessionFactoryName));
                }
                lock (this)
                {
                    if (!Factories.ContainsKey(sessionFactoryName)) //double check, it may use in multi-thread.
                    {
                        Add(nhFileConfig.SessionFactoryName, nhFileConfig.BuildSessionFactory());
                    }
                }
            }
            else
            {
                nhFileConfig = NhConfigManager.GetNhConfig(sessionFactoryName);
            }
            if (nhFileConfig != null && nhFileConfig.IsChanged)
            {
                Factories[sessionFactoryName] = nhFileConfig.BuildSessionFactory();
            }
            return Factories[sessionFactoryName];
        }

        public bool TryGetSessionFactory(Type entityType, out ISessionFactory sessionFactory)
        {
            //1st step to find default sessionFactoryKey.
            string entityName = entityType.UnderlyingSystemType.FullName;
            sessionFactory = null;
            bool result = GetSessionFactory(CurrentSessionFactoryKey).Statistics.EntityNames.Contains(
                entityName);
            if (result)
            {
                sessionFactory = GetSessionFactory(CurrentSessionFactoryKey);
                return true;
            }

            foreach (string key in Factories.Keys)
            {
                if (key == CurrentSessionFactoryKey)
                    continue; //skip the default 
                if (Factories[key].Statistics.EntityNames.Contains(entityName))
                {
                    sessionFactory = Factories[key];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        /// <exception cref="SessionException">Can't find entity type in nhibernate setting.</exception>
        public ISessionFactory GetSessionFactory(Type entityType)
        {
            ISessionFactory factory;
            if (!TryGetSessionFactory(entityType, out factory))
            {
                throw new SessionException(string.Format("can't find the mapping entity with {0}", entityType));
            }
            return factory;
        }

        /// <summary>
        ///  Close session. if submitData Specials, it will
        /// </summary>
        /// <param name="factories"></param>
        /// <param name="submitData"> </param>
        public void CleanUp(ISessionFactory[] factories, bool submitData)
        {
            if (factories == null) throw new ArgumentNullException("factories");
            foreach (ISessionFactory sf in factories)
            {
                if (CurrentSessionContext.HasBind(sf))
                {
                    ISession session = CurrentSessionContext.Unbind(sf);
                    HandleUnsaveData(submitData, session);
                    session.Close();
                }
            }
        }

        private static void HandleUnsaveData(bool submitData, ISession session)
        {
            if (submitData)
            {
                session.Flush();
            }
            else
            {
                session.Clear();
            }

            if (session.Transaction != null && session.Transaction.IsActive)
            {
                if (submitData && !session.Transaction.WasCommitted)
                {
                    session.Transaction.Commit();
                }
                else if (!session.Transaction.WasRolledBack)
                {
                    session.Transaction.Rollback();
                }
            }
        }
        /// <summary>
        /// close all session, if submitData is true, it will submit data to .
        /// </summary>
        /// <param name="submitData"></param>
        public void CleanUp(bool submitData)
        {
            CleanUp(Factories.Values.ToArray(), submitData);
            IsInitiated = false;
        }

        /// <summary>
        ///  Close session after rollback transaction if it actived and clear first-level cache.
        /// </summary>
        public void CleanUp()
        {
            CleanUp(Factories.Values.ToArray(), false);
            IsInitiated = false;
        }
    }
}
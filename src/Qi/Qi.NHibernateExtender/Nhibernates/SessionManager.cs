using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Stat;

namespace Qi.Nhibernates
{
    public class SessionManager : IDisposable
    {
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
            foreach (string a in NhConfigManager.SessionFactoryNames)
            {
                if (CurrentSessionFactoryKey == null || a == "default")
                    DefaultSessionFactoryKey = a;
                Factories.Add(a, NhConfigManager.GetNhConfig(a).BuildSessionFactory());
            }
        }

        /// <summary>
        /// 默认SessionManager
        /// </summary>
        private SessionManager()
        {
        }

        public static string CurrentSessionFactoryKey
        {
            get
            {
                return (CallContext.GetData("default_sessionFactoryName") as string) ?? DefaultSessionFactoryKey;
            }
            set
            {
                if (!Factories.ContainsKey(value))
                    throw new ArgumentOutOfRangeException("value", "Can't find the session factory name " + value + " in the setting.");
                CallContext.SetData("default_sessionFactoryName", value);
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
        }

        #endregion

        public static void Add(string sessionName, ISessionFactory config)
        {
            if (Factories.ContainsKey(sessionName))
            {
                throw new ArgumentNullException("session factory name " + sessionName + " has defined.");
            }
            Factories.Add(sessionName, config);
        }

        public static void Remove(string sessionName)
        {
            if (!Factories.ContainsKey(sessionName))
            {
                throw new ArgumentNullException("session factory name " + sessionName + " is not defined.");
            }
            Factories.Remove(sessionName);
        }

        /// <summary>
        /// Gets Session after use <see cref="IniSession"/>
        /// </summary>
        public ISession GetCurrentSession()
        {
            ISessionFactory sf = GetSessionFactory(CurrentSessionFactoryKey);

            if (!CurrentSessionContext.HasBind(sf))
            {
                if (((SessionFactoryImpl)sf).Name.Contains("testdata"))
                {
                    int a = 0;
                }
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


        public ISessionFactory GetSessionFactory(string sfName)
        {
            return Factories[sfName];
        }

        public ISessionFactory GetSessionFactory(Type entityType)
        {
            foreach (ISessionFactoryImplementor sf in Factories.Values)
            {
                if (sf.GetEntityPersister(entityType.FullName) != null)
                    return sf;
            }
            throw new SessionException("can't find the mapping entity with " + entityType);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>初始化了Session 返回true，如果调用这个方法之前已经初始化，返回false</returns>
        public bool IniSession()
        {
            return true;
        }
    }
}
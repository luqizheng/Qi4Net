using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Impl;
using Qi.SharePools;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public class SessionWrapper : IDisposable
    {
        private const string InitKeyName = "session.was.inited";
        private ISessionFactory _sessionFactory;
        private IStore _store;

        /// <summary>
        /// </summary>
        /// <param name="configruation"></param>
        public SessionWrapper(Configuration configruation)
        {
            if (configruation == null)
                throw new ArgumentNullException("configruation");
            Configuration = configruation;
        }

        /// <summary>
        /// </summary>
        public Configuration Configuration { get; private set; }

        /// <summary>
        /// </summary>
        public IStore Store
        {
            get { return _store ?? (_store = GetStore(SessionFactory)); }
        }

        /// <summary>
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory ?? (_sessionFactory = Configuration.BuildSessionFactory()); }
        }

        /// <summary>
        /// </summary>
        public ISession CurrentSession
        {
            get
            {
                if (!CurrentSessionContext.HasBind(SessionFactory))
                {
                    CurrentSessionContext.Bind(SessionFactory.OpenSession());
                }
                return SessionFactory.GetCurrentSession();
            }
        }

        /// <summary>
        /// </summary>
        public bool IsInitSession
        {
            get
            {
                object obj = Store.GetData(InitKeyName);
                if (obj == null)
                    return false;
                return (bool) obj;
            }
            private set { Store.SetData(InitKeyName, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool OpenInThisCurrent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// <code>
        /// 
        /// </code>
        /// </remarks>
        public void Dispose()
        {
            if (OpenInThisCurrent)
            {
                Close(true);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool InitSession()
        {
            bool result = false;
            if (!IsInitSession)
            {
                result = true;
                IsInitSession = true;
            }
            OpenInThisCurrent = result;
            return result;
        }

        /// <summary>
        ///     关闭Session 而且不管这个Session是否在当前CallContext中打开
        /// </summary>
        /// <param name="submitData"></param>
        public void Close(bool submitData)
        {
            if (CurrentSessionContext.HasBind(SessionFactory))
            {
                ISession session = CurrentSessionContext.Unbind(SessionFactory);
                HandleUnsaveData(submitData, session);
                session.Close();
            }
        }

        private static void HandleUnsaveData(bool submitData, ISession session)
        {
            if (submitData)
            {
                session.Flush();
            }
            if (session.Transaction != null && session.Transaction.IsActive && !session.Transaction.WasCommitted)
            {
                if (submitData)
                {
                    session.Transaction.Commit();
                }
                else
                {
                    session.Transaction.Rollback();
                }
            }
            session.Clear();
        }

        private static IStore GetStore(ISessionFactory sessionFactory)
        {
            var a = sessionFactory as SessionFactoryImpl;
            ICurrentSessionContext context = a.CurrentSessionContext;
            if (context is CallSessionContext)
            {
                return SharePool.CallContextStore;
            }
            if (context is ThreadStaticSessionContext)
            {
                return SharePool.ThreadStaticStore;
            }
            if (context is WebSessionContext)
            {
                return SharePool.HttpStore;
            }
            throw new SessionManagerException("Can't create save context base on " + context.GetType().Name);
        }

        /// <summary>
        ///     Rebuild the sessionFactory.
        /// </summary>
        public void Configure()
        {
            _sessionFactory = null;
        }
    }
}
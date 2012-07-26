using System;
using NHibernate;
using NHibernate.Context;
using NHibernate.Impl;
using Qi.SharePools;

namespace Qi.Nhibernates
{
    public class SessionSegment
    {
        private const string InitKeyName = "session.was.inited";
        private IStore _store;

        public SessionSegment(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");
            SessionFactory = sessionFactory;
        }

        public IStore Store
        {
            get { return _store ?? (_store = GetStore(SessionFactory)); }
        }

        public ISessionFactory SessionFactory { get; private set; }

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

        public bool InitSession()
        {
            bool result = false;
            if (!IsInitSession)
            {
                result = true;
                IsInitSession = true;
            }
            return result;
        }

        public void CleanUp(bool submitData)
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
    }
}
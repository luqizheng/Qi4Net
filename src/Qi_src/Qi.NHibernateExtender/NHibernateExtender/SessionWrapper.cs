using System;
using System.Data;
using NHibernate;
using NHibernate.Context;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public class SessionWrapper : IDisposable
    {
        private readonly ISession _session;
        private ISessionFactory _sessionFactory;


        /// <summary>
        /// </summary>
        /// <param name="session"></param>
        internal SessionWrapper(ISession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            _session = session;
            OpenInThisContext = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="openInThisContext"></param>
        internal SessionWrapper(ISessionFactory sessionFactory, bool openInThisContext)
        {
            if (sessionFactory == null)
            {
                throw new ArgumentNullException("sessionFactory");
            }
            SessionFactory = sessionFactory;
            OpenInThisContext = openInThisContext;
        }

        /// <summary>
        ///     个Wrapper是不是使用SessionContext进行bind。
        /// </summary>
        public bool BindToSessionContext
        {
            get { return _sessionFactory != null; }
        }

        /// <summary>
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get
            {
                if (BindToSessionContext)
                {
                    return _sessionFactory;
                }
                return _session.SessionFactory;
            }
            private set { _sessionFactory = value; }
        }

        /// <summary>
        ///     这个Wrapper是不是在这里关闭
        /// </summary>
        public bool OpenInThisContext { get; private set; }


        /// <summary>
        /// </summary>
        public ISession CurrentSession
        {
            get { return CurrentSessionProxy; }
        }

        internal SessionProxy CurrentSessionProxy
        {
            get
            {
                if (BindToSessionContext)
                {
                    return (SessionProxy) SessionFactory.GetCurrentSession();
                }
                return (SessionProxy) _session;
            }
        }

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            if (CurrentSessionProxy.IsOpen)
            {
                Close();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            _session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// </summary>
        public void BeginTransaction()
        {
            _session.BeginTransaction();
        }

        /// <summary>
        /// </summary>
        /// <param name="submitData"></param>
        /// <param name="session"></param>
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

        /// <summary>
        ///     关闭这个Wrapper，但是不一定关闭NSession，如果这个Session
        ///     并不是是在这里开启的，那么这个Close方法指挥提交数据，而不是关闭Session
        /// </summary>
        /// <param name="submit"></param>
        [Obsolete]
        public bool Close(bool submit)
        {
            SessionProxy session = CurrentSessionProxy;
            if (OpenInThisContext)
            {
                if (BindToSessionContext)
                {
                    if (CurrentSessionContext.HasBind(SessionFactory))
                    {
                        session = (SessionProxy) CurrentSessionContext.Unbind(SessionFactory);
                    }
                    if (session.Parent != null && SessionFactory != null)
                    {
                        CurrentSessionContext.Bind(session.Parent);
                    }
                }
                HandleUnsaveData(submit, session);
                session.Close();
                return true;
            }
            HandleUnsaveData(submit, session);
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (OpenInThisContext)
            {
                SessionProxy session = CurrentSessionProxy;
                if (CurrentSessionContext.HasBind(SessionFactory))
                {
                    session = (SessionProxy) CurrentSessionContext.Unbind(SessionFactory);
                }
                session.Close();
            }
            return false;
        }

        /// <summary>
        /// </summary>
        public void Commit()
        {
            CurrentSessionProxy.Flush();

            if (CurrentSession.Transaction != null
                && CurrentSession.Transaction.IsActive
                && !CurrentSession.Transaction.WasCommitted)
            {
                CurrentSession.Transaction.Commit();
            }
        }

        /// <summary>
        /// </summary>
        public void Rollback()
        {
            if (CurrentSession.Transaction != null
                && CurrentSession.Transaction.IsActive
                && !CurrentSession.Transaction.WasRolledBack)
            {
                CurrentSession.Transaction.Rollback();
            }
        }
    }
}
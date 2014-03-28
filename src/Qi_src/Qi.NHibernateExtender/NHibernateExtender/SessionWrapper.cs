using System;
using NHibernate;
using NHibernate.Context;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public class SessionWrapper : IDisposable
    {
        private readonly SessionProxy _session;

        internal SessionWrapper(SessionProxy session, ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
            _session = session;
        }
        /// <summary>
        /// 
        /// </summary>
        public ISessionFactory SessionFactory { get; private set; }


        /// <summary>
        /// </summary>
        public ISession CurrentSession
        {
            get { return _session; }
        }


        public void Dispose()
        {
            Close(false);
            CurrentSessionContext.Unbind(SessionFactory);
            if (_session.Parent != null)
            {
                CurrentSessionContext.Bind(_session.Parent);
            }
            _session.Dispose();
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="submit"></param>
        public void Close(bool submit)
        {
            HandleUnsaveData(submit, CurrentSession);
        }
    }
}
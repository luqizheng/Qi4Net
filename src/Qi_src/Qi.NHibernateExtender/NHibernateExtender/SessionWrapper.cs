using System;
using NHibernate;
using NHibernate.Context;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public class SessionWrapper : IDisposable
    {


        internal SessionWrapper(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;

        }

        /// <summary>
        /// </summary>
        public ISessionFactory SessionFactory { get; private set; }

        public bool OpenInThisContext
        {
            get
            {
                if (CurrentSessionProxy.Parent == null) //如果没有Parent，那么需要自己关闭
                {
                    return true;
                }
                if (CurrentSessionProxy.NHSession != CurrentSessionProxy.Parent.NHSession) //如果NHSession 和 Parent不是同一个
                {
                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// </summary>
        public ISession CurrentSession
        {
            get { return CurrentSessionProxy; }
        }

        internal SessionProxy CurrentSessionProxy
        {
            get { return (SessionProxy)this.SessionFactory.GetCurrentSession(); }
        }


        public void Dispose()
        {
            Close(false);
            CurrentSessionContext.Unbind(SessionFactory);
            if (CurrentSessionProxy.Parent != null)
            {
                CurrentSessionContext.Bind(CurrentSessionProxy.Parent);
            }
            CurrentSessionProxy.Dispose();
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
        /// </summary>
        /// <param name="submit"></param>
        public void Close(bool submit)
        {
            HandleUnsaveData(submit, CurrentSession);
        }
    }
}
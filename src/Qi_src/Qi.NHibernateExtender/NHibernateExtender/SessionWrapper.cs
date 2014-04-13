using System;
using NHibernate;
using NHibernate.Context;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public class SessionWrapper : IDisposable
    {

        internal SessionWrapper(ISessionFactory sessionFactory, bool openInThisContext)
        {
            SessionFactory = sessionFactory;
            OpenInThisContext = openInThisContext;
        }

        /// <summary>
        /// </summary>
        public ISessionFactory SessionFactory { get; private set; }

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
            get { return (SessionProxy)SessionFactory.GetCurrentSession(); }
        }


        public void Dispose()
        {
            Close(false);
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
        ///     关闭这个Wrapper，但是不一定关闭NSession，如果这个Session
        ///     并不是是在这里开启的，那么这个Close方法指挥提交数据，而不是关闭Session
        /// </summary>
        /// <param name="submit"></param>
        public bool Close(bool submit)
        {
            if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                return false;
            }

            SessionProxy session = CurrentSessionProxy;
            if (OpenInThisContext)
            {
                session = (SessionProxy)CurrentSessionContext.Unbind(SessionFactory);
                HandleUnsaveData(submit, session);
                session.Close();
                if (session.Parent != null)
                {
                    CurrentSessionContext.Bind(session.Parent);
                }
                return true;
            }
            HandleUnsaveData(submit, session);
            return false;

        }
    }
}
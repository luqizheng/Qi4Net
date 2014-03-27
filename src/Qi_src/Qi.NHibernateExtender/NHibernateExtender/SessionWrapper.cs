using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using Qi.Web.Mvc;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public class SessionWrapper : IDisposable
    {
        private readonly ISessionFactory _sessionFactory;
        private ISession _session;
        /// <summary>
        /// </summary>
        /// <param name="configruation"></param>
        /// <param name="sessionFactory"></param>
        public SessionWrapper(Configuration configruation, ISessionFactory sessionFactory)
        {
            if (configruation == null)
                throw new ArgumentNullException("configruation");
            Configuration = configruation;
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// </summary>
        public Configuration Configuration { get; private set; }

        /// <summary>
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        /// <summary>
        /// </summary>
        public ISession CurrentSession
        {
            get
            {
                if (!CurrentSessionContext.HasBind(SessionFactory))
                {
                    throw new NHModelBinderException("Please call InitSession first.");
                }

                ISession session = SessionFactory.GetCurrentSession();
                return session;
            }
        }

        /// <summary>
        /// </summary>
        [Obsolete]
        public bool IsInitSession
        {
            get
            {
                return CurrentSessionContext.HasBind(SessionFactory);
            }
        }

        /// <summary>
        /// </summary>
        public bool OpenInThisCurrent { get; private set; }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     <code>
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
            if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                _session = new MultiSession(SessionFactory.OpenSession());
                CurrentSessionContext.Bind(_session);
                OpenInThisCurrent = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool InitNewSession()
        {
            if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                CurrentSessionContext.Bind(SessionFactory.OpenSession());
                OpenInThisCurrent = true;
                return true;
            }
            else
            {
                
                var session = SessionFactory.OpenSession()
                session.
            }
            
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
    }
}
using System;
using NHibernate;
using Qi.NHibernateExtender;

namespace Qi.Domain.NHibernates
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractDao
    {
        private string _sessionFactoryName;
        private SessionWrapper _wrapper;
        private bool _managerBySelf = false;

        /// <summary>
        /// </summary>
        /// <param name="sessionWrapper"></param>
        protected AbstractDao(SessionWrapper sessionWrapper)
        {
            if (sessionWrapper == null)
                throw new ArgumentNullException("sessionWrapper");
            _managerBySelf = true;
            _wrapper = sessionWrapper;
        }

        protected AbstractDao(string sessionFactoryName)
        {
            if (String.IsNullOrEmpty(sessionFactoryName))
                throw new ArgumentNullException("sessionFactoryName");
            _sessionFactoryName = sessionFactoryName;
        }

        /// <summary>
        /// </summary>
        protected AbstractDao()
            : this(SessionManager.DefaultSessionFactoryKey)
        {

        }

        /// <summary>
        ///     Session Wrapper
        /// </summary>
        protected SessionWrapper SessionWrapper
        {
            get
            {
                if (_managerBySelf)
                {
                    return _wrapper;
                }
                var session = SessionManager.GetSessionWrapper(_sessionFactoryName);
                if (!session.CurrentSession.IsOpen)
                    throw new SessionManagerException("Please call SessionManager.GetSessionWrapper first.");
                return session;
            }
        }

        /// <summary>
        /// </summary>
        protected ISession CurrentSession
        {
            get { return SessionWrapper.CurrentSession; }
        }
    }
}
using System;
using System.Data;
using System.Web.Mvc;
using NHibernate;
using Qi.NHibernateExtender;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class SessionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private readonly SessionWrapper _wrapper;
        private ITransaction _tras;

        /// <summary>
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="sessionFactoryName"></param>
        public SessionAttribute(bool enabled, string sessionFactoryName)
        {
            Order = 0;
            Enable = enabled;
            SessionFactoryName = sessionFactoryName;
            _wrapper = SessionManager.GetSessionWrapper(sessionFactoryName);
        }

        /// <summary>
        /// </summary>
        /// <param name="enabled"></param>
        public SessionAttribute(bool enabled)
            : this(enabled, SessionManager.DefaultSessionFactoryKey)
        {
        }

        /// <summary>
        /// </summary>
        public SessionAttribute()
            : this(true, SessionManager.DefaultSessionFactoryKey)
        {
            Order = 0;
        }

        /// <summary>
        ///     Enable session or not.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        ///     session factory
        /// </summary>
        public string SessionFactoryName { get; set; }

        /// <summary>
        ///     Gets or sets the value indecate use transaction or not.
        /// </summary>
        public bool Transaction { get; set; }

        /// <summary>
        ///     Gets or sets the IsolationLevel, default use the config setting.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        #region IExceptionFilter Members

        /// <summary>
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            if (_tras != null && _tras.IsActive)
            {
                _tras.Rollback();
                _tras = null;
            }
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Enable)
            {
                _wrapper.InitSession();
                SessionManager.Instance.CurrentSessionFactoryName = SessionFactoryName;

                if (Transaction)
                {
                    _tras = IsolationLevel != null
                                ? _wrapper.CurrentSession.BeginTransaction(IsolationLevel.Value)
                                : _wrapper.CurrentSession.BeginTransaction();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            _wrapper.Close(true);
        }
    }
}
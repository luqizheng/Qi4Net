using System;
using System.Data;
using System.Web.Mvc;
using NHibernate;
using Qi.Nhibernates;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SessionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private readonly bool _enabled;
        private ITransaction _tras;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="sessionFactoryName"></param>
        public SessionAttribute(bool enabled, string sessionFactoryName)
            : this(enabled)
        {
            SessionFactoryName = sessionFactoryName ?? SessionManager.Instance.Config.SessionFactoryName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        public SessionAttribute(bool enabled)
        {
            Order = 0;
            _enabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public SessionAttribute()
            : this(true, null)
        {
            Order = 0;
        }

        public string SessionFactoryName { get; private set; }

        /// <summary>
        /// Gets or sets the value indecate use transaction or not.
        /// </summary>
        public bool Transaction { get; set; }

        /// <summary>
        /// Gets or sets the IsolationLevel, default use the config setting.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        #region IExceptionFilter Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            if (_tras != null)
            {
                _tras.Rollback();
                _tras = null;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SessionManager manager = BuildSessionManager();
            if (_enabled)
            {
                manager.IniSession();
            }
            if (Transaction)
            {
                _tras = IsolationLevel != null
                            ? manager.CurrentSession.BeginTransaction(IsolationLevel.Value)
                            : manager.CurrentSession.BeginTransaction();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            BuildSessionManager().CleanUp();
        }


        private SessionManager BuildSessionManager()
        {
            SessionManager sessionManager = !String.IsNullOrEmpty(SessionFactoryName)
                                                ? SessionManager.GetInstance(SessionFactoryName)
                                                : SessionManager.Instance;

            return sessionManager;
        }
    }
}
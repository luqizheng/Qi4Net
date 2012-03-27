using System;
using System.Data;
using System.Threading;
using System.Web.Mvc;
using NHibernate;
using Qi.Nhibernates;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class SessionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private ITransaction _tras;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="sessionFactoryName"></param>
        public SessionAttribute(bool enabled, string sessionFactoryName)
        {
            Order = 0;
            Enable = enabled;
            SessionFactoryName = sessionFactoryName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        public SessionAttribute(bool enabled)
            : this(enabled, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public SessionAttribute()
            : this(true, null)
        {
            Order = 0;
        }

        /// <summary>
        /// Enable session or not.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// session factory 
        /// </summary>
        public string SessionFactoryName { get; set; }

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
            if (Enable)
            {
                if (!String.IsNullOrEmpty(SessionFactoryName))
                    SessionManager.CurrentSessionFactoryKey = SessionFactoryName;
            }
            if (Transaction && Enable)
            {
                _tras = IsolationLevel != null
                            ? SessionManager.Instance.GetCurrentSession().BeginTransaction(IsolationLevel.Value)
                            : SessionManager.Instance.GetCurrentSession().BeginTransaction();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            SessionManager.Instance.CleanUp();
        }
    }
}
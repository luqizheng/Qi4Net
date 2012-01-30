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
        private readonly string _sessionFactoryName;
        private bool _autoCloase;
        private ITransaction _tras;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="sessionFactoryName"></param>
        public SessionAttribute(bool enabled, string sessionFactoryName)
        {
            Order = 0;
            _enabled = enabled;
            _sessionFactoryName = sessionFactoryName;           
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
            if (_enabled)
            {
                if (!String.IsNullOrEmpty(_sessionFactoryName))
                    _autoCloase = SessionManager.GetInstance(_sessionFactoryName).IniSession();
                else
                    _autoCloase = SessionManager.Instance.IniSession();

            }
            if (Transaction)
            {
                if (IsolationLevel != null)
                    _tras = SessionManager.Instance.CurrentSession.BeginTransaction(IsolationLevel.Value);
                else
                    _tras = SessionManager.Instance.CurrentSession.BeginTransaction();

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (_autoCloase)
            {
                if (!String.IsNullOrEmpty(_sessionFactoryName))
                {
                    SessionManager.GetInstance(_sessionFactoryName).CleanUp();
                }
                SessionManager.Instance.CleanUp();
            }
        }
    }
}
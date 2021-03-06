using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using NHibernate;
using Qi.NHibernateExtender;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;
using IExceptionFilter = System.Web.Http.Filters.IExceptionFilter;

namespace Qi.Web.Http
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ApiSessionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private ITransaction _tras;
        private SessionWrapper _wrapper;
        private bool _setIsolative;
        private IsolationLevel _isolationLevel;

        /// <summary>
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="sessionFactoryName"></param>
        public ApiSessionAttribute(bool enabled, string sessionFactoryName)
        {
            Enable = enabled;
            SessionFactoryName = sessionFactoryName;
        }

        /// <summary>
        /// </summary>
        /// <param name="enabled"></param>
        public ApiSessionAttribute(bool enabled)
            : this(enabled, SessionManager.DefaultSessionFactoryKey)
        {
        }

        /// <summary>
        /// </summary>
        public ApiSessionAttribute()
            : this(true, SessionManager.DefaultSessionFactoryKey)
        {
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
        public IsolationLevel IsolationLevel
        {
            get { return _isolationLevel; }
            set
            {
                _setIsolative = true;
                _isolationLevel = value;
            }
        }

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
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            var task = new Task(s =>
            {
                var trans = (ITransaction)s;
                if (trans != null && trans.IsActive)
                {
                    trans.Rollback();
                    trans = null;
                }
            }, _tras);
            return task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (Enable)
            {
                _wrapper = SessionManager.GetSessionWrapper(SessionFactoryName);
                if (Transaction)
                {
                    _tras = _setIsolative
                        ? _wrapper.CurrentSession.BeginTransaction(IsolationLevel)
                        : _wrapper.CurrentSession.BeginTransaction();
                }
            }
            base.OnActionExecuting(actionContext);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (_wrapper != null)
            {
                _wrapper.Commit();
                _wrapper.Close();
            }
        }
    }
}
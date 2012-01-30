using System;
using System.Data;
using System.Web.Mvc;
using NHibernate;
using Qi.Nhibernates;

namespace Qi.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SessionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private readonly bool _enabled;
        private readonly string _sessionFactoryName;
        private bool _autoCloase;
        private ITransaction _tras;

        public SessionAttribute(bool enabled, string sessionFactoryName)
        {
            Order = 0;
            _enabled = enabled;
            _sessionFactoryName = sessionFactoryName;
            IsolationLevel = IsolationLevel.ReadCommitted;
        }

        public SessionAttribute()
            : this(true, null)
        {
            Order = 0;
        }

        public bool Transaction { get; set; }

        public IsolationLevel IsolationLevel { get; set; }

        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            if (_tras != null)
            {
                _tras.Rollback();
                _tras = null;
            }
        }

        #endregion

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
                _tras = SessionManager.Instance.CurrentSession.BeginTransaction(IsolationLevel);
            }
        }

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NHibernate.Criterion;
using Qi.NHibernateExtender;

namespace Qi.Web
{
    public class NHSessionHttpModule : IHttpModule
    {
        private SessionWrapper _wrapper;
        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.EndRequest += context_EndRequest;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            _wrapper.Close(true);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            _wrapper = SessionManager.GetSessionWrapper();
        }

        public void Dispose()
        {

        }
    }
}

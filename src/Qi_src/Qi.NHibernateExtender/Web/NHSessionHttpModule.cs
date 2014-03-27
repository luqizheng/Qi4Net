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
        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.EndRequest += context_EndRequest;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            SessionManager.ClassAll(true);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            SessionManager.GetSessionWrapper().InitSession();
        }

        public void Dispose()
        {

        }
    }
}

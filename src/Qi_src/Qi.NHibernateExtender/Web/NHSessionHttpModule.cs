using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NHibernate.Criterion;
using Qi.NHibernateExtender;

namespace Qi.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class NHSessionHttpModule : IHttpModule
    {
        private SessionWrapper _wrapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.EndRequest += context_EndRequest;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_EndRequest(object sender, EventArgs e)
        {
            _wrapper.SubmitData();
            _wrapper.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_BeginRequest(object sender, EventArgs e)
        {
            _wrapper = SessionManager.GetSessionWrapper();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }
    }
}

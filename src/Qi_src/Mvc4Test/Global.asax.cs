﻿using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Qi.NHibernate;
using Qi.Web.Mvc;

namespace Mvc4Test
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            ModelBinders.Binders.DefaultBinder = new NHModelBinder();

            AreaRegistration.RegisterAllAreas();

            foreach (string config in NhConfigManager.SessionFactoryNames)
            {
                Configuration nhConfiguration =
                    NhConfigManager.GetNhConfig(config).NHConfiguration;
                var create = new SchemaUpdate(nhConfiguration);


                create.Execute(true, true);
            }
        }
    }
}
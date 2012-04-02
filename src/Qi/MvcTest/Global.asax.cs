using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NHibernate.Tool.hbm2ddl;
using Qi.Nhibernates;
using log4net.Config;

namespace MvcTest
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            foreach (var config in NhConfigManager.SessionFactoryNames)
            {
                var nhConfiguration =
                    NhConfigManager.GetNhConfig(config).NHConfiguration;
                var create = new SchemaExport(nhConfiguration);
                create.Drop(true, true);

                create.Create(true, true);
            }
        }
    }
}
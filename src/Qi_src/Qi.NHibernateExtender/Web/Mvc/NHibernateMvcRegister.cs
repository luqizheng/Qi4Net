using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Qi.NHibernateExtender;
using Qi.Web.Mvc.NHMvcExtender;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// NHibernate 更改DefaultBiner为NHModelBiner，并且更新ValueProviderFactories
    /// </summary>
    public static class NHibernateMvcRegister
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Regist()
        {
            //NHibernate Extender
            ValueProviderFactories.Factories[1] = new NHFormValueProviderFactory();
            ValueProviderFactories.Factories[3] = new NHRouterDataProviderFactory();
            ValueProviderFactories.Factories[4] = new NHQueryValuePrivoderFactory();
            ExtenderModelType();
        }

        private static void ExtenderModelType()
        {
            ModelBinders.Binders.DefaultBinder = new NHModelBinder();

        }
    }
}

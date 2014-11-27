using System.Web.Mvc;
using Qi.Web.Mvc.NHMvcExtender;

namespace Qi.Web.Mvc
{
    /// <summary>
    ///     NHibernate 更改DefaultBiner为NHModelBiner，并且更新ValueProviderFactories
    /// </summary>
    public static class NHibernateMvcRegister
    {
        /// <summary>
        /// </summary>
        public static void Regist()
        {
            //new ValueProviderFactoryCollection { new ChildActionValueProviderFactory(), new FormValueProviderFactory(), new JsonValueProviderFactory(), 
            //new RouteDataValueProviderFactory(), new QueryStringValueProviderFactory(), new HttpFileCollectionValueProviderFactory(), new JQueryFormValueProviderFactory() };
            //NHibernate Extender
            ValueProviderFactories.Factories[1] = new NHFormValueProviderFactory();
            ValueProviderFactories.Factories[3] = new NHRouterDataProviderFactory();
            ValueProviderFactories.Factories[4] = new NHQueryValuePrivoderFactory();
            ValueProviderFactories.Factories[6] = new NHJQueryFormProviderFactory();

            ExtenderModelType();
        }

        private static void ExtenderModelType()
        {
            ModelBinders.Binders.DefaultBinder = new NHModelBinder();
        }
    }
}
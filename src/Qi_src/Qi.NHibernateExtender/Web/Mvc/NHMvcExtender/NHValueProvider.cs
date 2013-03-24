using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;
using Qi.NHibernateExtender;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// </summary>
    public class NHValueProvider : NameValueCollectionValueProvider
    {
        private readonly ControllerContext _controllerContext;

        /// <summary>
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="collection"></param>
        public NHValueProvider(ControllerContext controllerContext, NameValueCollection collection)
            : base(collection, CultureInfo.CurrentCulture)
        {
            _controllerContext = controllerContext;
        }


        public override ValueProviderResult GetValue(string key)
        {
            ValueProviderResult result = base.GetValue(key);
            if (result == null)
                return null;
            return new NHValueProviderResult(result,
                                             NHModelBinder.GetWrapper(_controllerContext));
        }


        public override ValueProviderResult GetValue(string key, bool skipValidation)
        {
            ValueProviderResult result = base.GetValue(key, skipValidation);
            if (result == null)
                return null;
            return new NHValueProviderResult(result,NHModelBinder.GetWrapper(_controllerContext));
        }
    }
}
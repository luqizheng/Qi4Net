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

        public NHValueProvider(ControllerContext controllerContext, NameValueCollection collection)
            : base(collection, CultureInfo.CurrentCulture)
        {
            _controllerContext = controllerContext;
        }


        public override ValueProviderResult GetValue(string key)
        {
            ValueProviderResult result = base.GetValue(key);
            return new NHValueProviderResult(result,
                                             (SessionWrapper)
                                             _controllerContext.RequestContext.HttpContext.Items["nhwrapper"]);
        }


        public override ValueProviderResult GetValue(string key, bool skipValidation)
        {
            ValueProviderResult result = base.GetValue(key, skipValidation);
            return new NHValueProviderResult(result,
                                             (SessionWrapper)
                                             _controllerContext.RequestContext.HttpContext.Items["nhwrapper"]);
        }
    }
}
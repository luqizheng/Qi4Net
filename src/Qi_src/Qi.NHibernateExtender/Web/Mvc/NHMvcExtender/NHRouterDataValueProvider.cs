using System.Globalization;
using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// 
    /// </summary>
    public class NHRouterDataValueProvider : DictionaryValueProvider<object>
    {
        private readonly ControllerContext _controllerContext;

        // RouteData should use the invariant culture since it's part of the URL, and the URL should be
        // interpreted in a uniform fashion regardless of the origin of a particular request.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        public NHRouterDataValueProvider(ControllerContext controllerContext)
            : base(controllerContext.RouteData.Values, CultureInfo.InvariantCulture)
        {
            _controllerContext = controllerContext;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override ValueProviderResult GetValue(string key)
        {
            var result= base.GetValue(key);
            if (result == null)
                return null;
            return new NHValueProviderResult(result, NHModelBinder.GetWrapper(_controllerContext));
        }

    }
}
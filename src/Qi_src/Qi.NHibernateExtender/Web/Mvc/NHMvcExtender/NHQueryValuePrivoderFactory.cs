using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// 
    /// </summary>
    public class NHQueryValuePrivoderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new NHQueryValueProvider(controllerContext);
        }
    }
}
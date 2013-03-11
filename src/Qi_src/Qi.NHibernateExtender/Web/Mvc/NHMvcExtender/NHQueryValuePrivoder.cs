using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    public class NHQueryValueProvider:NHValueProvider
    {
        public NHQueryValueProvider(ControllerContext controllerContext)
            : base(controllerContext, controllerContext.RequestContext.HttpContext.Request.QueryString)
        {
        }

        
    }
}
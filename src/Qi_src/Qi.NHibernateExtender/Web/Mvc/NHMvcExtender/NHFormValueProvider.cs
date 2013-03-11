using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    public class NHFormValueProvider : NHValueProvider
    {
        public NHFormValueProvider(ControllerContext controllerContext)
            : base(controllerContext, controllerContext.RequestContext.HttpContext.Request.Form)
        {
        }
    }
}
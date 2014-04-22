using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// 
    /// </summary>
    public class NHFormValueProvider : NHValueProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        public NHFormValueProvider(ControllerContext controllerContext)
            : base(controllerContext, controllerContext.RequestContext.HttpContext.Request.Form)
        {
        }
    }
}
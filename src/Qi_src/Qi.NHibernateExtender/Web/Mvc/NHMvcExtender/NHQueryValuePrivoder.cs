using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// 
    /// </summary>
    public class NHQueryValueProvider:NHValueProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        public NHQueryValueProvider(ControllerContext controllerContext)
            : base(controllerContext, controllerContext.RequestContext.HttpContext.Request.QueryString)
        {
        }

        
    }
}
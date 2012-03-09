using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Qi.Web.Mvc
{
    public class JsonContainerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;
            ParameterDescriptor[] parameterNames = filterContext.ActionDescriptor.GetParameters();
            foreach (ParameterDescriptor parameterName in parameterNames)
            {
                if (parameterName.ParameterType == typeof(JsonContainer))
                {
                    filterContext.ActionParameters[parameterName.ParameterName] =
                        JsonContainer.Create(request[parameterName.ParameterName]);
                }
            }


        }


    }
}
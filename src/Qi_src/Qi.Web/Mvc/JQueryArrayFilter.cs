using System;
using System.Web.Mvc;

namespace Qi.Web.Mvc
{
    public class JQueryArrayFilter : ActionFilterAttribute
    {
        private readonly string[] _postNames;

        public JQueryArrayFilter(params string[] postNames)
        {
            if (postNames == null)
                throw new ArgumentNullException("postNames");
            _postNames = postNames;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            foreach (string key in _postNames)
            {
                string aryKey = key + "[]";
                ValueProviderResult valueProviderResult = filterContext.Controller.ValueProvider.GetValue(aryKey);
                if (valueProviderResult == null)
                    continue;
                filterContext.ActionParameters[key] = valueProviderResult.RawValue;
            }
        }
    }
}
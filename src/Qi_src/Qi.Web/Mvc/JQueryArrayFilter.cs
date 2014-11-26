using System;
using System.CodeDom;
using System.Web.Mvc;

namespace Qi.Web.Mvc
{
    public class JQueryArrayFilter : ActionFilterAttribute
    {
        private readonly string[] _postNames;

        public JQueryArrayFilter()
        {

        }
        public JQueryArrayFilter(params string[] postNames)
        {
            if (postNames == null)
                throw new ArgumentNullException("postNames");
            _postNames = postNames;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (_postNames != null)
            {
                foreach (string key in _postNames)
                {
                    string aryKey = key + "[]";
                    ValueProviderResult valueProviderResult = filterContext.Controller.ValueProvider.GetValue(aryKey);
                    if (valueProviderResult == null)
                        continue;
                    filterContext.ActionParameters[key] = valueProviderResult.RawValue;
                }
            }
            else
            {
                foreach (var key in filterContext.HttpContext.Request.Params.AllKeys)
                {
                    if (key.EndsWith("]"))
                    {
                        ValueProviderResult valueProviderResult = filterContext.Controller.ValueProvider.GetValue(key);
                        if (valueProviderResult == null)
                            continue;
                        var newKey = RemoveArraySufix(key);
                        filterContext.ActionParameters[newKey] = valueProviderResult.RawValue;
                    }
                }
            }
        }

        private string RemoveArraySufix(string key)
        {
            var sta = key.LastIndexOf('[');
            return key.Substring(0, sta);

        }
    }
}
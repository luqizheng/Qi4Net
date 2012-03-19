using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Qi.Web.Mvc
{
    public class JsonContainerFilter : ActionFilterAttribute
    {
        private const string formSubmit = "application/x-www-form-urlencoded";
        private readonly bool _submitDirectJquery;

        public JsonContainerFilter(bool submitDirectJquery)
        {
            _submitDirectJquery = submitDirectJquery;
        }

        public JsonContainerFilter()
            : this(false)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
            ParameterDescriptor[] parameterNames = filterContext.ActionDescriptor.GetParameters();

            if (!_submitDirectJquery)
            {
                foreach (ParameterDescriptor parameterName in parameterNames)
                {
                    if (parameterName.ParameterType == typeof(JsonContainer))
                    {
                        filterContext.ActionParameters[parameterName.ParameterName] =
                            JsonContainer.Create(request[parameterName.ParameterName]);
                    }
                }
            }
            else
            {
                IList<string> actionParameterName = new List<string>();

                foreach (ParameterDescriptor parameterName in parameterNames)
                {
                    if (parameterName.ParameterType == typeof(JsonContainer))
                    {
                        actionParameterName.Add(parameterName.ParameterName);
                    }
                }
                Dictionary<string, JsonContainer> jquerySet = MakeJsonContainer(request, actionParameterName);
                foreach (string key in jquerySet.Keys)
                {
                    filterContext.ActionParameters[key] = jquerySet[key];
                }
            }
        }

        public Dictionary<string, JsonContainer> MakeJsonContainer(HttpRequestBase request,
                                                                  IEnumerable<string> parameterName)
        {
            var result = new Dictionary<string, JsonContainer>();
            bool isPost = request.HttpMethod.ToLower() == "post";
            foreach (string requestKey in isPost ? request.Form.AllKeys : request.QueryString.AllKeys)
            {
                var val = isPost ? request.Form.GetValues(requestKey) : request.QueryString.GetValues(requestKey);
                RebuildRequestKey(parameterName, result, val, requestKey);
            }



            return result;
        }

        private void RebuildRequestKey(IEnumerable<string> actionParameterNames,
                                       Dictionary<string, JsonContainer> result, string[] val, string requestKey)
        {
            foreach (string parameterName in actionParameterNames)
            {
                if (requestKey.StartsWith(parameterName + "["))
                {
                    if (!result.ContainsKey(parameterName))
                    {
                        result.Add(parameterName, new JsonContainer());
                    }

                    string jsonContainerKeyPath = JsonKeyPath(requestKey.TrimStart(parameterName.ToCharArray()));

                    result[parameterName].SetValue(jsonContainerKeyPath, val);
                }
            }
        }


        public static string JsonKeyPath(string requestKey)
        {
            return Regex.Replace(requestKey, @"\[[A-z_]+\d*\]", s => "." + s.Value.Substring(1, s.Value.Length - 2));
        }
    }
}
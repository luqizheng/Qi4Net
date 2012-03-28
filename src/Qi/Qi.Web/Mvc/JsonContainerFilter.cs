using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Qi.Web.Mvc
{
    public class JsonContainerFilter : ActionFilterAttribute
    {
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
                NameValueCollection requestKeySet = request.HttpMethod.ToLower() == "post"
                                                        ? request.Form
                                                        : request.QueryString;
                if (requestKeySet.Keys[0] != null)
                {
                    foreach (ParameterDescriptor parameterName in parameterNames)
                    {
                        if (parameterName.ParameterType == typeof (JsonContainer))
                        {
                            filterContext.ActionParameters[parameterName.ParameterName] =
                                JsonContainer.Create(requestKeySet[parameterName.ParameterName]);
                        }
                    }
                }
                else
                {
                    filterContext.ActionParameters[parameterNames[0].ParameterName] =
                        JsonContainer.Create(requestKeySet[0]);
                }
            }
            else
            {
                var jquerySet = new Dictionary<string, JsonContainer>();
                foreach (ParameterDescriptor parameterName in parameterNames)
                {
                    if (parameterName.ParameterType == typeof (JsonContainer))
                    {
                        var json = new JsonContainer();
                        jquerySet.Add(parameterName.ParameterName, json);
                        filterContext.ActionParameters[parameterName.ParameterName] = json;
                    }
                }
                MakeJsonContainer(request, jquerySet);
            }
        }

        public void MakeJsonContainer(HttpRequestBase request, Dictionary<string, JsonContainer> jquerySet)
        {
            NameValueCollection requestKeySet = request.HttpMethod.ToLower() == "post"
                                                    ? request.Form
                                                    : request.QueryString;
            foreach (string requestKey in requestKeySet.AllKeys)
            {
                string[] val = requestKeySet.GetValues(requestKey);
                RebuildRequestKey(jquerySet, val, requestKey);
            }
        }

        private void RebuildRequestKey(Dictionary<string, JsonContainer> result, string[] val, string requestKey)
        {
            foreach (string parameterName in result.Keys)
            {
                if (requestKey.StartsWith(parameterName + "["))
                {
                    if (!result.ContainsKey(parameterName))
                    {
                        result.Add(parameterName, new JsonContainer());
                    }
                    string keyPath = requestKey.TrimStart(parameterName.ToCharArray());
                    string jsonContainerKeyPath = ToJsonContainerExpress(keyPath);

                    result[parameterName].SetValue(jsonContainerKeyPath.TrimStart('.'), val);
                }
            }
        }


        public static string ToJsonContainerExpress(string requestKey)
        {
            return Regex.Replace(requestKey, @"\[[A-z_]+\d*\]", s => "." + s.Value.Substring(1, s.Value.Length - 2));
        }
    }
}
using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Qi.Web.Mvc
{
    public static class HtmlHelper
    {
        public static MvcHtmlString LabelForModel<T, TValue>(this HtmlHelper<T> helper, T model,
                                                             Expression<Func<T, TValue>> expression)
        {
            return helper.LabelFor(expression);
        }
    }
}
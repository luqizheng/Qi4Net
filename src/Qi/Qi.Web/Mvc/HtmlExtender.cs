using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Qi.Web.Mvc
{
    public static class HtmlExtender
    {
        ///// <summary>
        ///// 
        ///// </summary>
        /////<remarks>
        ///// code from http://weblogs.asp.net/imranbaloch/archive/2010/07/03/asp-net-mvc-labelfor-helper-with-htmlattributes.aspx
        ///// </remarks>
        //public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //                                                     Expression<Func<TModel, TValue>> expression,
        //                                                     object htmlAttributes)
        //{
        //    return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        /////<remarks>
        ///// code from http://weblogs.asp.net/imranbaloch/archive/2010/07/03/asp-net-mvc-labelfor-helper-with-htmlattributes.aspx
        ///// </remarks>
        //public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //                                                     Expression<Func<TModel, TValue>> expression,
        //                                                     IDictionary<string, object> htmlAttributes)
        //{
        //    ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
        //    string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
        //    string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
        //    if (String.IsNullOrEmpty(labelText))
        //    {
        //        return MvcHtmlString.Empty;
        //    }

        //    var tag = new TagBuilder("label");
        //    tag.MergeAttributes(htmlAttributes);
        //    tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
        //    tag.SetInnerText(labelText);
        //    return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        //}
        /// <summary>
        /// Out put the expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString ContentFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                               Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }
            return MvcHtmlString.Create(labelText);
        }
    }
}
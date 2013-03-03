using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace NHibernateMvc4Test.Models
{
    public static class MVC4BugFix
    {
        public static MvcHtmlString BugFix(this MvcHtmlString s)
        {
            string result = Regex.Replace(s.ToString(), @"\.\[\d*\]", match => match.Value.Substring(1));
            return new MvcHtmlString(result);
        }
    }
}
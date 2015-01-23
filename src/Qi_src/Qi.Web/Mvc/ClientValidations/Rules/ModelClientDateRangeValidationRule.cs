using System.Web.Mvc;

namespace Qi.Web.Mvc.ClientValidations.Rules
{
    public class ModelClientDateRangeValidationRule : ModelClientValidationRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="min">for jquery +1y等模式</param>
        /// <param name="max"></param>
        /// <param name="clientDateFormat"></param>
        /// <param name="maxDate">for validation plugin 使用的</param>
        /// <param name="minDate"></param>
        public ModelClientDateRangeValidationRule(string errorMessage, string min, string max, string clientDateFormat,string maxDate,string minDate)
        {
            ErrorMessage = errorMessage;
            ValidationType = "rangedate";
            ValidationParameters["max"] = max;
            ValidationParameters["min"] = min;
            ValidationParameters["dateformat"] = clientDateFormat;
            ValidationParameters["mindate"] = minDate;
            ValidationParameters["maxdate"] = maxDate;
            
        }

        public string Max
        {
            get { return ValidationParameters["max"].ToString(); }
        }

        public string Min
        {
            get { return ValidationParameters["min"].ToString(); }
        }

        public string ClientDateFormat
        {
            get { return (string) ValidationParameters["dateformat"]; }
        }
    }
}
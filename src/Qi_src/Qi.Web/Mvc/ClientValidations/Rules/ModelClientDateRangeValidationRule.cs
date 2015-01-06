using System.Web.Mvc;

namespace Qi.Web.Mvc.ClientValidations.Rules
{
    public class ModelClientDateRangeValidationRule : ModelClientValidationRule
    {
        public ModelClientDateRangeValidationRule(string errorMessage, string min, string max, string clientDateFormat)
        {
            ErrorMessage = errorMessage;
            ValidationType = "rangedate";
            ValidationParameters["max"] = max;
            ValidationParameters["min"] = min;
            ValidationParameters["dateformat"] = clientDateFormat;
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
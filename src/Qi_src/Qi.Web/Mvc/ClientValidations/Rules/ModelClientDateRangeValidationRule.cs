using System;
using System.Web.Mvc;

namespace Qi.Web.Mvc.ClientValidations.Rules
{
    public class ModelClientDateRangeValidationRule : ModelClientValidationRule
    {
        public ModelClientDateRangeValidationRule(string errorMessage, DateTime min, DateTime max)
        {
            ErrorMessage = errorMessage;
            ValidationType = "rangeDate";
            ValidationParameters["Max"] = max;
            ValidationParameters["Min"] = min;
        }
    }
}
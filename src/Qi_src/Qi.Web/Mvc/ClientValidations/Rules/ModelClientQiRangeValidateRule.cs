using System.Web.Mvc;

namespace Qi.Web.Mvc.ClientValidations.Rules
{
    public class ModelClientQiRangeValidateRule : ModelClientValidationRangeRule
    {
        public ModelClientQiRangeValidateRule(string errorMessage, object minValue, object maxValue, object step)
            : base(errorMessage, minValue, maxValue)
        {
            ValidationParameters["Step"] = step;
        }
    }
}
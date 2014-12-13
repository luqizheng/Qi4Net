using System.Collections.Generic;
using System.Web.Mvc;
using Qi.Domain.Attributes;
using Qi.Web.Mvc.ClientValidations.Rules;

namespace Qi.Web.Mvc.ClientValidations.Adapters
{
    public class DateRangeAdapter : DataAnnotationsModelValidator<DateRangeAttribute>
    {
        public DateRangeAdapter(ModelMetadata metadata, ControllerContext context,
            DateRangeAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            string errorMessage = ErrorMessage;
            var displayFormat = Metadata.DisplayFormatString;
            return new[]
            {
                new ModelClientDateRangeValidationRule(errorMessage, 
                    Attribute.MinDate.ToString(displayFormat), Attribute.MaxDate.ToString(displayFormat),Attribute.ClientDateFormat)
            };
        }
    }
}
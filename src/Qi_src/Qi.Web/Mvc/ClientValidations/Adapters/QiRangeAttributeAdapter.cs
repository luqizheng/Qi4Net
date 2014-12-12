using System.Collections.Generic;
using System.Web.Mvc;
using Qi.Domain.Attributes;
using Qi.Web.Mvc.ClientValidations.Rules;

namespace Qi.Web.Mvc.ClientValidations.Adapters
{
    public class QiRangeAttributeAdapter : DataAnnotationsModelValidator<QiRangeAttribute>
    {
        public QiRangeAttributeAdapter(ModelMetadata metadata, ControllerContext context, QiRangeAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            string errorMessage = ErrorMessage;
            return new[] { new ModelClientQiRangeValidateRule(errorMessage, Attribute.Minimum, Attribute.Maximum, Attribute.Step) };
        }
    }
}
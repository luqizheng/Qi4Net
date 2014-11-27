using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    public class NHJQueryFormProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new NHJQueryFormProvider(controllerContext);
        }

    }
}

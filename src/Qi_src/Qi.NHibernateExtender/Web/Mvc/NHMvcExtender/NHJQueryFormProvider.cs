using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// 
    /// </summary>
    public class NHJQueryFormProvider : JQueryFormValueProvider
    {
        private readonly ControllerContext _controllerContext;

        public NHJQueryFormProvider(ControllerContext controllerContext)
            : base(controllerContext)
        {
            _controllerContext = controllerContext;
        }

        public override ValueProviderResult GetValue(string key, bool skipValidation)
        {
            var result = base.GetValue(key,skipValidation);
            if (result == null)
                return null;
            return new NHValueProviderResult(result, NHModelBinder.GetWrapper(_controllerContext));
        }

        public override ValueProviderResult GetValue(string key)
        {
            var result =  base.GetValue(key);
            if (result == null)
                return null;
            return new NHValueProviderResult(result, NHModelBinder.GetWrapper(_controllerContext));
        }
    }
}

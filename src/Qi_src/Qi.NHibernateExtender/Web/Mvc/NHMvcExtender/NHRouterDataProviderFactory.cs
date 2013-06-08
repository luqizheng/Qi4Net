using System;
using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// 
    /// </summary>
    public class NHRouterDataProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            return new NHRouterDataValueProvider(controllerContext);
        }
    }
}
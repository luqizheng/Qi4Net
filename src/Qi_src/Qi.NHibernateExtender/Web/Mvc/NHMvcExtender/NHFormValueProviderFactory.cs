using System.Web.Mvc;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// 
    /// </summary>
    public class NHFormValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new NHFormValueProvider(controllerContext);
        }
    }
}
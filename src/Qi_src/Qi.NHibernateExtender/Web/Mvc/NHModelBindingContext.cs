using System.Web.Mvc;
using Qi.NHibernateExtender;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public class NHModelBindingContext : ModelBindingContext
    {
        /// <summary>
        /// 
        /// </summary>
        public ModelBindingContext Context { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public NHModelBindingContext(ModelBindingContext context)
        {
            Context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        public SessionWrapper Wrapper { get; set; }


    }
}
using System;
using System.Linq;
using System.Web.Mvc;
using Qi.NHibernateExtender;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// </summary>
    /// <remarks>
    ///     Regist it to application_start
    ///     <code>  
    ///  ModelBinderProviders.BinderProviders.Regist(new NHModelBinderProvider());
    /// </code>
    /// </remarks>
    public class NHModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public IModelBinder GetBinder(Type modelType)
        {
            foreach (string key in SessionManager.Factories.Keys)
            {
                if (
                    SessionManager.Factories[key].SessionFactory.Statistics.EntityNames.Contains(
                        modelType.UnderlyingSystemType.FullName))
                {
                    return new NHModelBinder();
                }
            }
            return null;
        }
    }
}
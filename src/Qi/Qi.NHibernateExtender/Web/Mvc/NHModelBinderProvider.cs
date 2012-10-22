using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Qi.NHibernate;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Regist it to application_start
    /// <code>  
    ///  ModelBinderProviders.BinderProviders.Add(new NHModelBinderProvider());
    /// </code>
    /// </remarks>
    public class NHModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            foreach (var key in SessionManager.Factories.Keys)
            {
                if (SessionManager.Factories[key].SessionFactory.Statistics.EntityNames.Contains(modelType.UnderlyingSystemType.FullName))
                {
                    return new NHModelBinder();
                }
            }
            return null;
        }
    }
}

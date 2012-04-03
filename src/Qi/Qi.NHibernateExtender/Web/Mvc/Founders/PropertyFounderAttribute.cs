using System;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Mapping;
using NHibernate.Type;
using Qi.Nhibernates;
using Property = NHibernate.Mapping.Property;

namespace Qi.Web.Mvc.Founders
{
    public class PropertyFounderAttribute : FounderAttribute
    {
        private readonly string _propertyName;

        /// <summary>
        /// default is the request's propertyName,
        /// </summary>
        public PropertyFounderAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public PropertyFounderAttribute(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            _propertyName = propertyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="postData"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override object GetObject(ISession session, object postData, string postName,
                                            HttpContextBase context)
        {
            DetachedCriteria cri = DetachedCriteria.For(EntityType).Add(Restrictions.Eq(_propertyName, postData));
            if (Unique)
            {
                var result = cri.SetFirstResult(0).SetMaxResults(1).GetExecutableCriteria(session).List();
                return result.Count > 0 ? result[0] : null;
            }
            return cri.GetExecutableCriteria(session).List();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="postDataName"></param>
        /// <returns></returns>
        /// <exception cref="NhConfigurationException">can found the property in class.</exception>
        protected override IType PostDataType(ISession session, string postDataName)
        {
            string propertyName = postDataName;
            if (!string.IsNullOrEmpty(_propertyName))
                propertyName = _propertyName;
            Configuration nh = NhConfigManager.GetNhConfig(SessionManager.CurrentSessionFactoryKey).NHConfiguration;
            PersistentClass mappingInfo = nh.GetClassMapping(EntityType);
            Property property = mappingInfo.GetProperty(propertyName);
            if (property == null)
                throw new NhConfigurationException(string.Format("cannot find property {0} in class {1} ", propertyName,
                                                                 mappingInfo.ClassName));
            return property.Type;
        }
    }
}
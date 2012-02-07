using System;
using System.Web;
using NHibernate.Criterion;
using NHibernate.Mapping;
using NHibernate.Type;
using Qi.Nhibernates;
using Property = NHibernate.Mapping.Property;

namespace Qi.Web.Mvc.Founders
{
    public class PropertyFounderAttribute : FounderAttribute
    {
        private string _propertyName;

        /// <summary>
        /// default is the request's propertyName,
        /// </summary>
        public PropertyFounderAttribute()
        {
        }

        public PropertyFounderAttribute(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            _propertyName = propertyName;
        }

        protected override object GetObject(SessionManager sessionManager, object postData, string postName,
                                            HttpContextBase context)
        {
            DetachedCriteria cri = DetachedCriteria.For(EntityType).Add(Restrictions.Eq(_propertyName, postData));
            if (!Unique)
                cri.SetFirstResult(0).SetMaxResults(1);
            return cri.GetExecutableCriteria(sessionManager.CurrentSession);
        }

        protected override IType PostDataType(SessionManager sessionManager, string postDataName)
        {
            if (!string.IsNullOrEmpty(_propertyName))
                _propertyName = postDataName;

            PersistentClass mappingInfo = sessionManager.Config.NHConfiguration.GetClassMapping(EntityType);
            Property property = mappingInfo.GetProperty(_propertyName);
            if (property == null)
                throw new NhConfigurationException(string.Format("cannot find property {0} in class {1} ", _propertyName,
                                                                 mappingInfo.ClassName));
            return property.Type;
        }
    }
}
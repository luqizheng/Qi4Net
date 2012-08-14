using System;
using System.Collections;
using System.Collections.Specialized;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Metadata;
using NHibernate.Type;
using Qi.NHibernate;

namespace Qi.Web.Mvc.Founders
{
    /// <summary>
    /// Use the Propertry equal value to find the mapping class.
    /// </summary>
    public class PropertyFounderAttribute : FounderAttribute
    {
        private readonly string _propertyName;

        /// <summary>
        /// default is the request's propertyName,
        /// </summary>
        public PropertyFounderAttribute()
        {
            Unique = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public PropertyFounderAttribute(string propertyName)
            : this()
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            _propertyName = propertyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="searchConditionValue"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override IList GetObject(ISession session, object[] searchConditionValue, string postName,
                                           NameValueCollection context)
        {
            DetachedCriteria cri =
                DetachedCriteria.For(EntityType);
            var disjunction = new Disjunction();
            foreach (object condition in searchConditionValue)
            {
                disjunction.Add(Restrictions.Eq(_propertyName, condition));
            }
            cri.Add(disjunction);
            if (Unique)
            {
                cri.SetFirstResult(0).SetMaxResults(1);
            }
            return cri.GetExecutableCriteria(session).List();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestKey"></param>
        /// <returns></returns>
        /// <exception cref="NhConfigurationException">can found the property in class.</exception>
        public override IType GetMappingType(ISession session, string requestKey)
        {
            string propertyName = string.IsNullOrEmpty(_propertyName) ? requestKey : _propertyName;
            IClassMetadata mappingInfo = session.SessionFactory.GetClassMetadata(EntityType);
            IType propertyType = mappingInfo.GetPropertyType(propertyName);

            if (propertyType == null)
                throw new NhConfigurationException(string.Format("cannot find property {0} in class {1} ", propertyName,
                                                                 mappingInfo.EntityName));
            return propertyType;
        }
    }
}
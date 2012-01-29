using System;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Mapping;
using NHibernate.Type;
using Qi.Nhibernates;
using Property = NHibernate.Mapping.Property;

namespace Qi.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NhModelFounderAttribute : Attribute
    {
        private readonly string _hql;
        private readonly string _mappingPropertyName;
        private readonly bool _uniqueResult;

        public NhModelFounderAttribute(string mappingPropertyName, bool uniqueResult)
        {
            if (mappingPropertyName == null) throw new ArgumentNullException("mappingPropertyName");
            _mappingPropertyName = mappingPropertyName;
            _uniqueResult = uniqueResult;
        }

        public NhModelFounderAttribute(bool uniqueResult, string hql)
        {
            _hql = hql;
            _uniqueResult = uniqueResult;
        }

        /// <summary>
        /// Use persistent's id for searching.
        /// </summary>
        public NhModelFounderAttribute()
        {
        }

        public object Find(SessionManager session, string propertyStrVal, Type entityType)
        {
            bool result = session.IniSession();
            try
            {
                PersistentClass mappingInfo = session.Config.NHConfiguration.GetClassMapping(entityType);

                if (String.IsNullOrEmpty(_mappingPropertyName)) // use id to get object
                {
                    object idValue = ConvertObject(propertyStrVal, mappingInfo.Identifier.Type);
                    return session.CurrentSession.Load(entityType, idValue);
                }
                if (!String.IsNullOrEmpty(_hql))
                {
                    IQuery crit = session.CurrentSession.CreateQuery(_hql);
                    return _uniqueResult ? crit.UniqueResult() : crit.List();
                }
                return FindByMappingProperty(session, propertyStrVal, entityType, mappingInfo);
            }
            finally
            {
                if (result)
                {
                    session.CleanUp();
                }
            }
        }

        private object FindByMappingProperty(SessionManager session, string propertyStrVal, Type entityType,
                                             PersistentClass mappingInfo)
        {
            Property property = mappingInfo.GetProperty(_mappingPropertyName);
            object propertyValue = ConvertObject(propertyStrVal, property.Type);
            ICriteria crit = DetachedCriteria.For(entityType).Add(
                Restrictions.Eq(_mappingPropertyName, propertyValue)).GetExecutableCriteria(
                    session.CurrentSession);
            return _uniqueResult ? crit.UniqueResult() : crit.List();
        }

        private static object ConvertObject(string id, IType type)
        {
            var idType = type as NullableType;
            if (idType == null)
                throw new NhConfigurationException(
                    "Resource's Id only support mapping from  NullableType in nhibernate.");
            return idType.FromStringValue(id);
        }
    }
}
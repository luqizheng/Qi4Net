using System;
using System.Web.Mvc;
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
        private readonly string[] _anohterHqlType;
        private readonly string[] _anotherHqlName;
        private readonly string _hql;
        private readonly string _hqlParameterType;
        private readonly string _mappingPropertyName;
        private readonly bool _uniqueResult;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappingPropertyName">property of the mapping class</param>
        /// <param name="uniqueResult">result is unique or not</param>
        public NhModelFounderAttribute(string mappingPropertyName, bool uniqueResult)
        {
            if (mappingPropertyName == null) throw new ArgumentNullException("mappingPropertyName");
            _mappingPropertyName = mappingPropertyName;
            _uniqueResult = uniqueResult;
        }

        /// <summary>
        /// use hql to found the object, submit value will set it to first place.
        /// </summary>
        /// <param name="hql">hql, the Parameter should be same as model's Property Name</param>
        /// <param name="nhibernateType">NHibernate string Type such as String,Int32,Int64,Guid,Decimal,Decimal(19,2)</param>
        public NhModelFounderAttribute(string hql, string nhibernateType)
            : this(true, hql, nhibernateType, null, null)
        {
        }

        /// <summary>
        /// use hql to creating the Founder.
        /// </summary>
        /// <param name="uniqueResult"></param>
        /// <param name="hql"></param>
        /// <param name="parameterType">the type of post data</param>
        /// <param name="anotherHqlName"> </param>
        /// <param name="anohterHqlType">NHibernate string Type such as String,Int32,Int64,Guid,Decimal,Decimal(19,2)</param>
        public NhModelFounderAttribute(bool uniqueResult, string hql, string parameterType, string[] anotherHqlName,
                                       string[] anohterHqlType)
        {
            _hql = hql;
            _uniqueResult = uniqueResult;
            _hqlParameterType = parameterType;
            _anotherHqlName = anotherHqlName;
            _anohterHqlType = anohterHqlType;
        }

        /// <summary>
        /// Use persistent's id for searching.
        /// </summary>
        public NhModelFounderAttribute()
        {
        }

        public object Find(SessionManager session, string propertyName, string propertyStrVal, Type entityType,
                           ControllerContext context)
        {
            bool result = session.IniSession();
            try
            {
                PersistentClass mappingInfo = session.Config.NHConfiguration.GetClassMapping(entityType);

                if (!String.IsNullOrEmpty(_mappingPropertyName)) // use id to get object
                {
                    //use Property to find the entity.
                    IType type = mappingInfo.GetProperty(_mappingPropertyName).Type;
                    object idValue = ConvertStringToObject(propertyStrVal, type);
                    return session.CurrentSession.Load(entityType, idValue);
                }
                if (!String.IsNullOrEmpty(_hql))
                {
                    //use hql to get the object;
                    return GetObjectByHql(session, propertyStrVal, propertyName, context);
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

        private object GetObjectByHql(SessionManager session, string propertyStrVal, string propertyName,
                                      ControllerContext context)
        {
            IQuery crit = session.CurrentSession.CreateQuery(_hql);

            IType hqlType = TypeFactory.Basic(_hqlParameterType);

            object inputParameter = ConvertStringToObject(propertyStrVal, hqlType);
            crit.SetParameter(propertyName, inputParameter, hqlType);
            if (_anohterHqlType != null)
            {
                int i = 1;
                foreach (string paramer in _anotherHqlName)
                {
                    string strValu = context.RequestContext.HttpContext.Request[paramer];

                    IType paramType = TypeFactory.Basic(_anohterHqlType[i - 1]);
                    object val = ConvertStringToObject(strValu, paramType);

                    crit.SetParameter(paramer, val, paramType);
                    i++;
                }
            }

            return _uniqueResult ? crit.UniqueResult() : crit.List();
        }

        private object FindByMappingProperty(SessionManager session, string propertyStrVal, Type entityType,
                                             PersistentClass mappingInfo)
        {
            Property property = mappingInfo.GetProperty(_mappingPropertyName);
            object propertyValue = ConvertStringToObject(propertyStrVal, property.Type);
            ICriteria crit = DetachedCriteria.For(entityType).Add(
                Restrictions.Eq(_mappingPropertyName, propertyValue)).GetExecutableCriteria(
                    session.CurrentSession);
            return _uniqueResult ? crit.UniqueResult() : crit.List();
        }

        private static object ConvertStringToObject(string valStrExpress, IType type)
        {
            var idType = type as NullableType;
            if (idType == null)
                throw new NhConfigurationException(
                    "Resource's Id only support mapping from NullableType in nhibernate.");
            return idType.FromStringValue(valStrExpress);
        }
    }
}
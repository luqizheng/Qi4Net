﻿using System;
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
        private readonly object[] _anotherHqlValue;
        private readonly string _hql;
        private readonly string _hqlParameterType;
        private readonly string _mappingPropertyName;
        private readonly int _parameterTypePosition;
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
        /// <param name="hql"></param>
        /// <param name="nhibernateType">NHibernate string Type such as String,Int32,Int64,Guid,Decimal,Decimal(19,2)</param>
        public NhModelFounderAttribute(string hql, string nhibernateType)
            : this(true, hql, nhibernateType, 0, null, null)
        {
        }

        /// <summary>
        /// use hql to creating the Founder.
        /// </summary>
        /// <param name="uniqueResult"></param>
        /// <param name="hql"></param>
        /// <param name="parameterTypePosition"> </param>
        /// <param name="anotherHqlValue"> </param>
        /// <param name="anohterHqlType">NHibernate string Type such as String,Int32,Int64,Guid,Decimal,Decimal(19,2)</param>
        public NhModelFounderAttribute(bool uniqueResult, string hql, string parameterType, int parameterTypePosition,
                                       object[] anotherHqlValue, string[] anohterHqlType)
        {
            _hql = hql;
            _uniqueResult = uniqueResult;
            _hqlParameterType = parameterType;
            _parameterTypePosition = parameterTypePosition;
            _anotherHqlValue = anotherHqlValue;
            _anohterHqlType = anohterHqlType;
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
                    //use Property to find the name.
                    object idValue = ConvertStringToObject(propertyStrVal, mappingInfo.Identifier.Type);
                    return session.CurrentSession.Load(entityType, idValue);
                }
                if (!String.IsNullOrEmpty(_hql))
                {
                    //use hql to get the object;
                    return GetObjectByHql(session, propertyStrVal);
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

        private object GetObjectByHql(SessionManager session, string propertyStrVal)
        {
            IQuery crit = session.CurrentSession.CreateQuery(_hql);

            IType hqlType = TypeFactory.Basic(_hqlParameterType);

            object inputParameter = ConvertStringToObject(propertyStrVal, hqlType);
            crit.SetParameter(_parameterTypePosition, inputParameter, hqlType);
            if (_anohterHqlType != null)
            {
                int i = 1;
                foreach (object val in _anotherHqlValue)
                {
                    IType paramType = TypeFactory.Basic(_anohterHqlType[i - 1]);
                    crit.SetParameter(i, val, paramType);
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
                    "Resource's Id only support mapping from  NullableType in nhibernate.");
            return idType.FromStringValue(valStrExpress);
        }
    }
}
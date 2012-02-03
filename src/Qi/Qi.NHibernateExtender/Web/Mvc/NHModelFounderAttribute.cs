using System;
using System.Web.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Mapping;
using NHibernate.Type;
using Qi.Nhibernates;
using Qi.Web.Mvc.Founders;
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

        private readonly FounderAttribute _founderAttribute;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappingPropertyName">property of the mapping class</param>
        /// <param name="uniqueResult">result is unique or not</param>
        public NhModelFounderAttribute(string mappingPropertyName, bool uniqueResult)
        {
            if (mappingPropertyName == null)
                throw new ArgumentNullException("mappingPropertyName");
            _founderAttribute = new PropertyFounderAttributeAttribute(mappingPropertyName)
                           {
                               Unique = uniqueResult
                           };

        }

        public NhModelFounderAttribute()
        {
            _founderAttribute = new PropertyFounderAttributeAttribute();
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
            _hqlParameterType = parameterType;
            _anotherHqlName = anotherHqlName;
            _anohterHqlType = anohterHqlType;
        }

        /// <summary>
        /// Use persistent's id for searching.
        /// </summary>
        public NhModelFounderAttribute()
        {
            this._founderAttribute = new IdFounderAttributeAttribute();
        }

        public object Find(SessionManager session, string propertyName, string propertyStrVal, Type entityType,
                           ControllerContext context)
        {
            bool result = session.IniSession();
            try
            {
                if (_founderAttribute.EntityType == null)
                {
                    _founderAttribute.EntityType = entityType;
                }

                return _founderAttribute.GetObject(session, propertyStrVal, propertyName, context.HttpContext);
            }
            finally
            {
                if (result)
                {
                    session.CleanUp();
                }
            }
        }






    }
}
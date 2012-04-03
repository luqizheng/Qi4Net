using System;
using System.Collections;
using System.Reflection;
using System.Web;
using NHibernate;
using NHibernate.Type;
using Qi.Nhibernates;

namespace Qi.Web.Mvc.Founders
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class FounderAttribute : Attribute
    {
        private Type _entityType;

        protected FounderAttribute()
        {
            Unique = true;
        }

        /// <summary>
        /// 标记查询结果集合是否为唯一，如果不是唯一，那么会拿第一个
        /// </summary>
        public bool Unique { get; protected set; }

        /// <summary>
        /// 获取或设置Entity的类型
        /// </summary>
        public Type EntityType
        {
            get { return _entityType; }
            set
            {
                if (_entityType == null)
                {
                    _entityType = value;
                }
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <param name="isSet"> </param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public IList GetObject(string postName, HttpContextBase context, bool isSet)
        {
            ISession session = SessionManager.Instance.GetCurrentSession();
            string requestValues = context.Request[postName];

            IType mappingType = GetMappingType(session, postName);


            if (context.Request[postName] != null)
            {
                object[] searchConditionValues = isSet
                                                     ? NHMappingHelper.ConvertStringToObjects(requestValues, mappingType)
                                                     : new[]
                                                           {
                                                               NHMappingHelper.ConvertStringToObject(requestValues, mappingType)
                                                           };

                return GetObject(SessionManager.Instance.GetCurrentSession(), searchConditionValues, postName, context);
            }
            ConstructorInfo constructor = EntityType
                .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                null, new Type[0], new ParameterModifier[0]);
            return new ArrayList(){constructor.Invoke(null)};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="id"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract IList GetObject(ISession session, object[] id, string postName, HttpContextBase context);

        /// <summary>
        /// 把Post的string类型的data，转换为IType类型,用于NHibernate获取对象的时候使用。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestKey"></param>
        /// <returns></returns>
        public abstract IType GetMappingType(ISession session, string requestKey);
    }
}
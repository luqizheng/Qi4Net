using System;
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
        public bool Unique { get; set; }

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
        /// <param name="postData"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object GetObject(string postData, string postName, HttpContextBase context)
        {
            ISession session = SessionManager.Instance.GetCurrentSession();

            try
            {
                if (context.Request[postName] != null)
                {
                    object postDataObj = NHMappingHelper.ConvertStringToObject(postData,
                                                                               PostDataType(session, postName));
                    return GetObject(SessionManager.Instance.GetCurrentSession(), postDataObj, postName, context);
                }
                else
                {
                    ConstructorInfo constructor = EntityType
                        .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, new Type[0], new ParameterModifier[0]);
                    return constructor.Invoke(null);
                }
            }
            finally
            {
                SessionManager.Instance.CleanUp();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="postData"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract object GetObject(ISession session, object postData, string postName,
                                            HttpContextBase context);

        /// <summary>
        /// 把Post的string类型的data，转换为ValueType类型，用于nhibernate的插叙
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="postDataName"></param>
        /// <returns></returns>
        protected abstract IType PostDataType(ISession session, string postDataName);
    }
}
using System;
using System.Web;
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
        /// <param name="sessionManager"></param>
        /// <param name="postData"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object GetObject(SessionManager sessionManager, string postData, string postName, HttpContextBase context)
        {
            bool result = sessionManager.IniSession();
            try
            {
                object postDataObj = ConvertStringToObject(postData, PostDataType(sessionManager, postName));
                return GetObject(sessionManager, postDataObj, postName, context);
            }
            finally
            {
                if (result)
                {
                    sessionManager.CleanUp();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valStrExpress"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected static object ConvertStringToObject(string valStrExpress, IType type)
        {
            var idType = type as NullableType;
            if (idType == null)
                throw new NhConfigurationException(
                    "Resource's Id only support mapping from NullableType in nhibernate.");
            return idType.FromStringValue(valStrExpress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="postData"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract object GetObject(SessionManager sessionManager, object postData, string postName,
                                            HttpContextBase context);

        /// <summary>
        /// 把Post的string类型的data，转换为ValueType类型，用于nhibernate的插叙
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="postDataName"></param>
        /// <returns></returns>
        protected abstract IType PostDataType(SessionManager sessionManager, string postDataName);
    }
}
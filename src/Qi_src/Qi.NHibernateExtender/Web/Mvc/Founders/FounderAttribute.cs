using System;
using System.Collections;
using System.Reflection;
using System.Web.Mvc;
using NHibernate;
using NHibernate.Type;

namespace Qi.Web.Mvc.Founders
{
    /// <summary>
    ///     Find the object whihc belong a property or field defined in a DTO
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class FounderAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        protected FounderAttribute()
        {
            Unique = true;
        }

        /// <summary>
        ///     标记查询结果集合是否为唯一，如果不是唯一，那么会拿第一个
        /// </summary>
        public bool Unique { get; protected set; }

        /// <summary>
        ///     获取或设置Entity的类型
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        ///     获取对象
        /// </summary>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <param name="isSet"> </param>
        /// <param name="session"> </param>
        /// <returns></returns>
        public IList GetObject(string postName, ModelBindingContext context, bool isSet, ISession session)
        {
            var result = context.ValueProvider.GetValue(postName);
            if (result != null)
            {
                var requestValues = (string)result.ConvertTo(typeof(string));
                IType mappingType = GetMappingType(session, postName);

                if (!string.IsNullOrEmpty(requestValues))
                {
                    try
                    {
                        object[] searchConditionValues = isSet
                                                             ? NHMappingHelper.ConvertStringToObjects(requestValues,
                                                                                                      mappingType)
                                                             : new[]
                                                                 {
                                                                     NHMappingHelper.ConvertStringToObject(
                                                                         requestValues,
                                                                         mappingType)
                                                                 };


                        return GetObject(session, searchConditionValues, postName,
                                         context);
                    }
                    catch (FormatException ex)
                    {
                        throw new NHModelBinderException(
                            "Translate submit data from client to target type (" + mappingType.Name + ") fail.", ex);
                    }
                }
            }

            ConstructorInfo constructor = EntityType
                .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                null, new Type[0], new ParameterModifier[0]);
            return new ArrayList { constructor.Invoke(null) };
        }

        /// <summary>
        /// </summary>
        /// <param name="session"></param>
        /// <param name="id"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract IList GetObject(ISession session, object[] id, string postName, ModelBindingContext context);

        /// <summary>
        ///     把Post的string类型的data，转换为IType类型,用于NHibernate获取对象的时候使用。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestKey"></param>
        /// <returns></returns>
        public abstract IType GetMappingType(ISession session, string requestKey);
    }
}
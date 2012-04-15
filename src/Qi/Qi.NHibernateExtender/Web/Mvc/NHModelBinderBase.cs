using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Qi.Collections;
using Qi.Web.Mvc.Founders;

namespace Qi.Web.Mvc
{
    public abstract class NHModelBinderBase : DefaultModelBinder
    {
        /// <summary>
        /// when binder object is nhibernate entity, it will set to true,
        /// </summary>
        private bool _isDto = true;

        protected abstract ISession CurrentSession(Type entityType);
        protected abstract ISessionFactory CurrentSessionFactory(Type entityType);

        protected NameValueCollection GetSubmitValues(HttpContextBase context)
        {
            return context.Request.HttpMethod.ToLower() == "post"
                       ? context.Request.Form
                       : context.Request.QueryString;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Initilize(controllerContext, bindingContext);

            return base.BindModel(controllerContext, bindingContext);
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            if (!IsPersistentType(modelType))
            {
                _isDto = true;
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
            _isDto = false;

            object result = GetObject(modelType, GetSubmitValues(controllerContext.HttpContext));
            if (result == null)
            {
                ConstructorInfo constructor = modelType
                    .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                    null, new Type[0], new ParameterModifier[0]);
                return constructor.Invoke(null);
            }
            return result;
        }

        /// <summary>
        /// Initilize session
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        protected abstract void Initilize(ControllerContext controllerContext, ModelBindingContext bindingContext);

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                             PropertyDescriptor propertyDescriptor)
        {
            if (!bindingContext.PropertyMetadata.ContainsKey(propertyDescriptor.Name))
            {
                return;
            }
            ModelMetadata propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            Type parameterType;
            Type modelType = propertyMetadata.ModelType;
            NameValueCollection context = GetSubmitValues(controllerContext.HttpContext);
            if (_isDto && SetActivtor.IsSupport(modelType, out parameterType))
            {
                //List  setting。只有dto才能set list，如果是Domainobject，那么会忽略这个list set
                ISessionFactory sessionFactory = CurrentSessionFactory(parameterType);
                if (sessionFactory == null)
                {
                    throw new ArgumentException("can't find the mapping type " + parameterType.FullName +
                                                " in session factory");
                }

                FounderAttribute founder = GetEntityFounderIn(propertyDescriptor.ComponentType, propertyDescriptor);
                founder.EntityType = parameterType;
                object children = CreateSetInstance(modelType, context, propertyDescriptor.Name,
                                                    parameterType, founder);
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, children);
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }

        private object CreateSetInstance(Type modelType, NameValueCollection context, string requestKey,
                                         Type parameterType,
                                         FounderAttribute founder)
        {
            if (context[requestKey] == null)
                return null;
            SetActivtor setHelper = SetActivtor.Create(modelType);
            int capcaity = NHMappingHelper.ConvertToArray(context[requestKey]).Length;
            object result = setHelper.Create(parameterType, capcaity);
            SetAccessor accessor = setHelper.CreateAccessor(result);
            bool isArray = result.GetType().IsArray;
            IList val = founder.GetObject(requestKey, context, true, CurrentSession(parameterType));
            if (founder.Unique)
            {
                if (isArray)
                    accessor.Set(val, 0);
                else

                    accessor.Add(val);

                return result;
            }

            int index = 0;
            foreach (object entity in val)
            {
                if (isArray)
                    accessor.Set(entity, index);
                else
                    accessor.Add(entity);
                index++;
            }
            return result;
        }

        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                            PropertyDescriptor propertyDescriptor, object value)
        {
            ModelMetadata propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            if (!IsPersistentType(propertyMetadata.ModelType))
            {
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
                return;
            }
            if (bindingContext.ModelState[propertyDescriptor.Name].Errors.Count != 0)
            {
                return;
            }
            // property is  persistent object,
            //bindingContext.ModelState[propertyDescriptor.Name].Errors.Clear();
            string modelStateKey = CreateSubPropertyName(bindingContext.ModelName, propertyMetadata.PropertyName);


            //skip Readonly properyt.
            if (!propertyDescriptor.IsReadOnly)
            {
                try
                {
                    value = GetObjectFrom(propertyDescriptor,
                                          GetSubmitValues(controllerContext.HttpContext), bindingContext.ModelType);
                    propertyDescriptor.SetValue(bindingContext.Model, value);
                }
                catch (Exception ex)
                {
                    // Only add if we're not already invalid
                    if (bindingContext.ModelState.IsValidField(modelStateKey))
                    {
                        bindingContext.ModelState.AddModelError(modelStateKey, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyDescriptor"></param>
        /// <param name="httpContextBase"> </param>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private object GetObjectFrom(PropertyDescriptor propertyDescriptor, NameValueCollection httpContextBase,
                                     Type modelType)
        {
            FounderAttribute founderAttribute = GetEntityFounderIn(modelType, propertyDescriptor);

            if (founderAttribute.EntityType == null)
            {
                founderAttribute.EntityType = propertyDescriptor.PropertyType;
            }
            IList result = founderAttribute.GetObject(propertyDescriptor.Name, httpContextBase, false,
                                                      CurrentSession(propertyDescriptor.PropertyType));
            return result.Count > 0 ? result[0] : null;
        }

        public bool IsPersistentType(Type modelType)
        {
            ISessionFactory sessionFactory = CurrentSessionFactory(modelType);

            if (sessionFactory != null)
                return true;

            if (modelType.IsArray)
            {
                sessionFactory = CurrentSessionFactory(modelType.GetElementType());

                if (sessionFactory != null)
                    return true;
                return
                    modelType.GetGenericArguments().Any(
                        type => CurrentSessionFactory(type) != null);
            }
            if (modelType.IsGenericType)
            {
                return
                    modelType.GetGenericArguments().Any(
                        type => CurrentSessionFactory(type) != null);
            }
            return false;
        }

        private static FounderAttribute GetEntityFounderIn(Type modelType, PropertyDescriptor propertyDescriptor)
        {
            object[] customAttributes =
                modelType.GetProperty(propertyDescriptor.Name).GetCustomAttributes(typeof(FounderAttribute), true);

            if (customAttributes.Length == 0)
            {
                return new IdFounderAttribute();
            }
            return (FounderAttribute)customAttributes[0];
        }

        /// <summary>
        /// if model is nh mapping class, use this found to getit.
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Object GetObject(Type modelType, NameValueCollection context)
        {
            var s = new IdFounderAttribute
                        {
                            EntityType = modelType
                        };
            string id = CurrentSessionFactory(modelType).GetClassMetadata(modelType).IdentifierPropertyName;

            IList result = s.GetObject(id, context, false, CurrentSession(modelType));
            return result.Count > 0 ? result[0] : null;
        }
    }
}
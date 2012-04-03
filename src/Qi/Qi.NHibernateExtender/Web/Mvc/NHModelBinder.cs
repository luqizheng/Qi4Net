using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Qi.Collections;
using Qi.Nhibernates;
using Qi.Web.Mvc.Founders;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// ModelBinder runs earlier than SessionAttribute, so this instance have to open Session by itself 
    /// and close it by SessionAttribute(because the entity may be lazy entity).
    /// if the controller has any SessionAttribute in actions or Controller, it have to close it in ActionEnding.
    /// </summary>
    public class NHModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// when binder object is nhibernate entity, it will set to true,
        /// </summary>
        private bool _isDto = true;

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            SessionManager.Instance.InitSession();
            return base.BindModel(controllerContext, bindingContext);
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            InitSessionAttribute(controllerContext, bindingContext);


            if (!IsPersistentType(modelType))
            {
                _isDto = true;
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
            _isDto = false;
            HttpRequestBase request = controllerContext.RequestContext.HttpContext.Request;
            object result = GeModelFromNH(modelType, request, controllerContext);
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
        /// Find Session Attribute in the Action or Controller.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        private void InitSessionAttribute(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var reflectedControllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());

            //find on Action, sessionAttribute 's priority on action is heiher than on controller.
            string actionname = controllerContext.RouteData.Values["Action"].ToString();
            ActionDescriptor action = reflectedControllerDescriptor.FindAction(controllerContext, actionname);

            //Find session attribute on the action.
            object[] customAttributes = action.GetCustomAttributes(typeof(SessionAttribute), true);
            if (customAttributes.Cast<SessionAttribute>().Any(s => s.Enable))
            {
                return;
            }

            //try to find attribute on Controller.
            customAttributes = controllerContext.Controller.GetType().GetCustomAttributes(typeof(SessionAttribute),
                                                                                          true);
            if (customAttributes.Cast<SessionAttribute>().Any(s => s.Enable))
            {
                return;
            }

            if (customAttributes.Length == 0)
                throw new NHModelBinderException(
                    "can't find any enabled SessionAttribute on controller or action ,please special session attribute and make sure it's enabled.");
        }

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

            if (_isDto && SetActivtor.IsSupport(modelType, out parameterType))
            {
                //List  setting。只有dto才能set list，如果是Domainobject，那么会忽略这个list set
                ISessionFactory sessionFactory;
                if (!SessionManager.Instance.TryGetSessionFactory(parameterType, out sessionFactory))
                {
                    throw new ArgumentException("can't find the mapping type " + parameterType.FullName + " in " +
                                                SessionManager.CurrentSessionFactoryKey);
                }

                FounderAttribute founder = GetEntityFounderIn(propertyDescriptor.ComponentType, propertyDescriptor);
                founder.EntityType = parameterType;
                object children = CreateSetInstance(modelType, controllerContext.HttpContext, propertyDescriptor.Name,
                                                    parameterType, founder);
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, children);
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }

        private object CreateSetInstance(Type modelType, HttpContextBase context, string requestKey, Type parameterType,
                                         FounderAttribute founder)
        {
            if (context.Request[requestKey] == null)
                return null;
            SetActivtor setHelper = SetActivtor.Create(modelType);
            int capcaity = NHMappingHelper.ConvertToArray(context.Request[requestKey]).Length;
            object result = setHelper.Create(parameterType, capcaity);
            SetAccessor accessor = setHelper.CreateAccessor(result);
            bool isArray = result.GetType().IsArray;
            IList val = founder.GetObject(requestKey, context, true);
            if (founder.Unique)
            {
                if (isArray)
                    accessor.Set(val, 0);
                else
                {
                    accessor.Add(val);
                }
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

            // property is  persistent object,
            bindingContext.ModelState[propertyDescriptor.Name].Errors.Clear();
            value = GetPersistentObject(controllerContext, propertyDescriptor, bindingContext.ModelType);
            string modelStateKey = CreateSubPropertyName(bindingContext.ModelName, propertyMetadata.PropertyName);

            //skip Readonly properyt.
            if (!propertyDescriptor.IsReadOnly)
            {
                try
                {
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
        /// <param name="controllerContext"></param>
        /// <param name="propertyDescriptor"></param>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private object GetPersistentObject(ControllerContext controllerContext, PropertyDescriptor propertyDescriptor,
                                           Type modelType)
        {
            HttpContextBase httpContextBase = controllerContext.RequestContext.HttpContext;

            FounderAttribute founderAttribute = GetEntityFounderIn(modelType, propertyDescriptor);

            if (founderAttribute.EntityType == null)
            {
                founderAttribute.EntityType = propertyDescriptor.PropertyType;
            }
            IList result = founderAttribute.GetObject(propertyDescriptor.Name, httpContextBase, false);
            return result.Count > 0 ? result[0] : null;
        }

        public static bool IsPersistentType(Type modelType)
        {
            ISessionFactory sessionFactory;
            SessionManager sessionManager = SessionManager.Instance;
            if (sessionManager.TryGetSessionFactory(modelType, out sessionFactory))
                return true;

            if (modelType.IsArray)
            {
                bool attach = sessionManager.TryGetSessionFactory(modelType.GetElementType(), out sessionFactory);
                if (attach)
                    return true;
                return
                    modelType.GetGenericArguments().Any(
                        type => sessionManager.TryGetSessionFactory(type, out sessionFactory));
            }
            if (modelType.IsGenericType)
            {
                return
                    modelType.GetGenericArguments().Any(
                        type => sessionManager.TryGetSessionFactory(type, out sessionFactory));
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
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual Object GeModelFromNH(Type modelType, HttpRequestBase request, ControllerContext context)
        {
            var s = new IdFounderAttribute
                        {
                            EntityType = modelType
                        };
            string id =
                SessionManager.Instance.GetSessionFactory(modelType).GetClassMetadata(modelType).IdentifierPropertyName;

            IList result = s.GetObject(id, context.HttpContext, false);
            return result.Count > 0 ? result[0] : null;
        }
    }

    public class NHModelBinderException : ApplicationException
    {
        public NHModelBinderException(string message)
            : base(message)
        {
        }
    }
}
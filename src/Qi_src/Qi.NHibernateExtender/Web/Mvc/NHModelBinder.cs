using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using NHibernate.Metadata;
using NHibernate.Type;
using Qi.NHibernateExtender;
using Qi.Web.Mvc.Founders;

namespace Qi.Web.Mvc
{
    /// <summary>
    ///     Nhibernate model biner
    /// </summary>
    public class NHModelBinder : DefaultModelBinder
    {
        private const string SessionWrapperContainer = "nhwrapper";
        private SessionWrapper _wrapper;

        /// <summary>
        ///     Binds the model by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        ///     The bound object.
        /// </returns>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param>
        /// <param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="bindingContext " />parameter is null.
        /// </exception>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var context = bindingContext as NHModelBindingContext;
            _wrapper = context == null ? Initilize(controllerContext) : context.Wrapper;

            if (_wrapper == null)
                return base.BindModel(controllerContext, bindingContext);

            bool addSuccess = SetWrapper(controllerContext, _wrapper);
            try
            {
                return base.BindModel(controllerContext, context == null ? bindingContext : context.Context);
            }
            finally
            {
                if (addSuccess)
                {
                    RemoveWrapper(controllerContext);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="wrapper"></param>
        internal
            static bool SetWrapper(ControllerContext controllerContext, SessionWrapper wrapper)
        {
            IDictionary items = controllerContext.RequestContext.HttpContext.Items;
            if (!items.Contains(SessionWrapperContainer))
            {
                items.Add(SessionWrapperContainer, wrapper);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remvoe NH Session, but it nevery close the session.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        internal static bool RemoveWrapper(ControllerContext controllerContext)
        {
            IDictionary items = controllerContext.RequestContext.HttpContext.Items;
            if (items.Contains(SessionWrapperContainer))
            {
                items.Remove(SessionWrapperContainer);
                return true;
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        internal static SessionWrapper GetWrapper(ControllerContext controllerContext)
        {
            IDictionary items = controllerContext.RequestContext.HttpContext.Items;
            if (items.Contains(SessionWrapperContainer))
            {
                return items[SessionWrapperContainer] as SessionWrapper;
            }
            return null;
        }

        /// <summary>
        ///     Creates the specified model type by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        ///     A data object of the specified type.
        /// </returns>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param>
        /// <param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param>
        /// <param name="modelType">The type of the model object to return.</param>
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            object result = IsMappingClass(modelType)
                                ? GetObjectById(modelType, bindingContext)
                                : base.CreateModel(controllerContext, bindingContext, modelType);

            if (result == null)
            {
                return CreateInstanceHelper.CreateInstance(modelType);
            }
            return result;
        }

        /// <summary>
        ///     Returns the value of a property using the specified controller context, binding context, property descriptor, and property binder.
        /// </summary>
        /// <returns>
        ///     An object that represents the property value.
        /// </returns>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param>
        /// <param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param>
        /// <param name="propertyDescriptor">The descriptor for the property to access. The descriptor provides information such as the component type, property type, and property value. It also provides methods to get or set the property value.</param>
        /// <param name="propertyBinder">An object that provides a way to bind the property.</param>
        protected override object GetPropertyValue(ControllerContext controllerContext,
                                                   ModelBindingContext bindingContext,
                                                   PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            var context = new NHModelBindingContext(bindingContext)
                {
                    Wrapper = _wrapper
                };
            object value = propertyBinder.BindModel(controllerContext, context);
            if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && Equals(value, String.Empty))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        ///     get a value to indecate the modelType or it's child element  is mapping class which defined in nhibernate.
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public bool IsPersistentType(Type modelType)
        {
            if (!modelType.IsArray && modelType.IsValueType)
                return false;

            var types = new List<Type> { modelType.IsArray ? modelType.GetElementType() : modelType };

            if (modelType.IsGenericType)
            {
                types.AddRange(modelType.GetGenericArguments());
            }

            return
                types.Any(
                    type => _wrapper.SessionFactory.Statistics.EntityNames.Contains(type.UnderlyingSystemType.FullName));
        }

        /// <summary>
        ///     return value  this modelType is belong to Mappling class or not.
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private bool IsMappingClass(Type modelType)
        {
            return _wrapper.SessionFactory.Statistics.EntityNames.Contains(modelType.UnderlyingSystemType.FullName);
        }


        /// <summary>
        ///     if model is nh mapping class, use this found to getit.
        /// </summary>
        /// <param name="mappingType"></param>
        /// <param name="bindingContext"> </param>
        /// <returns></returns>
        private Object GetObjectById(Type mappingType, ModelBindingContext bindingContext)
        {
            var idFounderAttribute = new IdFounderAttribute
                {
                    EntityType = mappingType
                };
            IClassMetadata perisisteType = _wrapper.SessionFactory.GetClassMetadata(mappingType);
            string idKey = CreateSubPropertyName(bindingContext.ModelName, perisisteType.IdentifierPropertyName);
            var identity = perisisteType.IdentifierType as PrimitiveType;
            if (identity != null)
            {
                var valurProvider = bindingContext.ValueProvider.GetValue(idKey);
                if (valurProvider == null)
                    return null;
                object postIdValue = valurProvider.ConvertTo(identity.DefaultValue.GetType());
                if (postIdValue.Equals(identity.DefaultValue))
                {
                    return null;
                }
            }
            var immutableId = perisisteType.IdentifierType as ImmutableType;
            if (immutableId != null)
            {
                idKey = CreateSubPropertyName(bindingContext.ModelName, perisisteType.IdentifierPropertyName);
                if (idKey == null)
                    return null;
            }

            IList result = idFounderAttribute.GetObject(idKey, bindingContext, false, _wrapper.CurrentSession);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        ///     Find Session Attribute in the Action or Controller.
        /// </summary>
        /// <param name="controllerContext"></param>
        private SessionWrapper Initilize(ControllerContext controllerContext)
        {
            var reflectedControllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());

            //find on Action, sessionAttribute 's priority on action is heiher than on controller.
            string actionname = controllerContext.RouteData.Values["Action"].ToString();
            ActionDescriptor action = reflectedControllerDescriptor.FindAction(controllerContext, actionname);

            //Find session attribute on the action.
            var customAttributeSet = new[]
                {
                    action.GetCustomAttributes(typeof (SessionAttribute), true)
                    , controllerContext.Controller.GetType().GetCustomAttributes(typeof (SessionAttribute), true)
                };
            SessionWrapper wrapper = null;
            if (customAttributeSet.Any(customAttributes => TryEnableSession(customAttributes, out wrapper)))
            {
                return wrapper;
            }
            return null;

            /*throw new NHModelBinderException(
                "can't find any enabled SessionAttribute on controller or action ,please special session attribute and make sure it's enabled.");*/
        }

        /// <summary>
        /// </summary>
        /// <param name="customAttributes"></param>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        private bool TryEnableSession(object[] customAttributes, out SessionWrapper wrapper)
        {
            wrapper = null;
            if (customAttributes.Length != 0)
            {
                var custommAttr = (SessionAttribute)customAttributes[0];
                if (custommAttr.Enable)
                {
                    wrapper = SessionManager.GetSessionWrapper(custommAttr.SessionFactoryName);
                    wrapper.InitSession();
                    return true;
                }
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
        ///     Create submit key of form/querystring
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal new static string CreateSubPropertyName(string prefix, string propertyName)
        {
            if (String.IsNullOrEmpty(prefix))
            {
                return propertyName;
            }
            if (String.IsNullOrEmpty(propertyName))
            {
                return prefix;
            }
            return prefix + "." + propertyName;
        }
    }
}
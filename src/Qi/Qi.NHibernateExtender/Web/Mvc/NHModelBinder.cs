using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Qi.Collections;
using Qi.NHibernate;
using Qi.Web.Mvc.Founders;

namespace Qi.Web.Mvc
{
    /// <summary>
    /// Nhibernate model biner 
    /// </summary>
    public class NHModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// when binder object is nhibernate entity, it will set to true,
        /// </summary>
        private bool _isDto = true;

        private SessionWrapper _wrapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private NameValueCollection GetSubmitValues(HttpContextBase context)
        {
            return context.Request.HttpMethod.ToLower() == "post"
                       ? context.Request.Form
                       : context.Request.QueryString;
        }

        /// <summary>
        /// Binds the model by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound object.
        /// </returns>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param><param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param><exception cref="T:System.ArgumentNullException">The <paramref name="bindingContext "/>parameter is null.</exception>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null) throw new ArgumentNullException("controllerContext");
            if (bindingContext == null) throw new ArgumentNullException("bindingContext");
            _wrapper = Initilize(controllerContext);
            return base.BindModel(controllerContext, bindingContext);
        }

        /// <summary>
        /// Creates the specified model type by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// A data object of the specified type.
        /// </returns>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param><param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param><param name="modelType">The type of the model object to return.</param>
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            _isDto = false;

            if (!IsMappingClass(modelType))
            {
                _isDto = true;
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }

            object result = GetObjectById(modelType, GetSubmitValues(controllerContext.HttpContext));
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
        /// Binds the specified property by using the specified controller context and binding context and the specified property descriptor.
        /// </summary>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param><param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param><param name="propertyDescriptor">Describes a property to be bound. The descriptor provides information such as the component type, property type, and property value. It also provides methods to get or set the property value.</param>
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                             PropertyDescriptor propertyDescriptor)
        {
            if (!bindingContext.PropertyMetadata.ContainsKey(propertyDescriptor.Name))
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
            Type modelType = bindingContext.PropertyMetadata[propertyDescriptor.Name].ModelType;
            Type parameterType;
            NameValueCollection context = GetSubmitValues(controllerContext.HttpContext);
            bool isListProperty = CollectionActivtor.IsSupport(modelType, out parameterType);

            if (isListProperty)
            {
                if (_isDto)
                {
                    FounderAttribute founder = GetEntityFounderIn(propertyDescriptor.ComponentType, propertyDescriptor);
                    founder.EntityType = parameterType;
                    object children = CreateSetInstance(modelType, context, propertyDescriptor.Name,
                                                        parameterType, founder);
                    base.SetProperty(controllerContext, bindingContext, propertyDescriptor, children);
                    return;
                }
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
                return;
            }
            if (IsPersistentType(modelType))
            {
                //set mapping class property.
                object value = GetObjectFrom(propertyDescriptor,
                                             GetSubmitValues(controllerContext.HttpContext), bindingContext.ModelType);
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
                return;
            }
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }

        private object CreateSetInstance(Type modelType, NameValueCollection context, string requestKey,
                                         Type parameterType,
                                         FounderAttribute founder)
        {
            if (context[requestKey] == null)
                return null;
            CollectionActivtor collectionHelper = CollectionActivtor.Create(modelType);
            int capcaity = NHMappingHelper.ConvertToArray(context[requestKey]).Length;
            object result = collectionHelper.Create(parameterType, capcaity);
            CollectionAccessor accessor = collectionHelper.CreateAccessor(result);
            bool isArray = result.GetType().IsArray;
            IList val = founder.GetObject(requestKey, context, true, _wrapper.CurrentSession);
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
                                                      _wrapper.CurrentSession);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        /// get a value to indecate the modelType or it's child element  is mapping class which defined in nhibernate.
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
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private bool IsMappingClass(Type modelType)
        {
            return _wrapper.SessionFactory.Statistics.EntityNames.Contains(modelType.UnderlyingSystemType.FullName);
        }


        /// <summary>
        /// if model is nh mapping class, use this found to getit.
        /// </summary>
        /// <param name="mappingType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private Object GetObjectById(Type mappingType, NameValueCollection context)
        {
            var idFounderAttribute = new IdFounderAttribute
                                         {
                                             EntityType = mappingType
                                         };
            string id = _wrapper.SessionFactory.GetClassMetadata(mappingType).IdentifierPropertyName;

            IList result = idFounderAttribute.GetObject(id, context, false, _wrapper.CurrentSession);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        /// Find Session Attribute in the Action or Controller.
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
                                             action.GetCustomAttributes(typeof (SessionAttribute), true),
                                             controllerContext.Controller.GetType().GetCustomAttributes(
                                                 typeof (SessionAttribute), true)
                                         };
            SessionWrapper wrapper = null;
            if (customAttributeSet.Any(customAttributes => TryEnableSession(customAttributes, out wrapper)))
            {
                return wrapper;
            }

            throw new NHModelBinderException(
                "can't find any enabled SessionAttribute on controller or action ,please special session attribute and make sure it's enabled.");
        }

        /// <summary>
        /// 
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
    }
}
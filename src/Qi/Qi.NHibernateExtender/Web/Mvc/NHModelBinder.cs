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
using Qi.Nhibernates;
using Qi.Web.Mvc.Founders;

namespace Qi.Web.Mvc
{
    public class NHModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// when binder object is nhibernate entity, it will set to true,
        /// </summary>
        private bool _isDto = true;

        private SessionSegment _segment;

        protected NameValueCollection GetSubmitValues(HttpContextBase context)
        {
            return context.Request.HttpMethod.ToLower() == "post"
                       ? context.Request.Form
                       : context.Request.QueryString;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            _segment = Initilize(controllerContext);
            return base.BindModel(controllerContext, bindingContext);
        }

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
            bool isListProperty = SetActivtor.IsSupport(modelType, out parameterType);

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
            SetActivtor setHelper = SetActivtor.Create(modelType);
            int capcaity = NHMappingHelper.ConvertToArray(context[requestKey]).Length;
            object result = setHelper.Create(parameterType, capcaity);
            SetAccessor accessor = setHelper.CreateAccessor(result);
            bool isArray = result.GetType().IsArray;
            IList val = founder.GetObject(requestKey, context, true, _segment.CurrentSession);
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
                                                      _segment.CurrentSession);
            return result.Count > 0 ? result[0] : null;
        }

        public bool IsPersistentType(Type modelType)
        {
            if (!modelType.IsArray && modelType.IsValueType)
                return false;

            var types = new List<Type> {modelType.IsArray ? modelType.GetElementType() : modelType};

            if (modelType.IsGenericType)
            {
                types.AddRange(modelType.GetGenericArguments());
            }

            return
                types.Any(
                    type => _segment.SessionFactory.Statistics.EntityNames.Contains(type.UnderlyingSystemType.FullName));
        }

        private bool IsMappingClass(Type modelType)
        {
            return _segment.SessionFactory.Statistics.EntityNames.Contains(modelType.UnderlyingSystemType.FullName));
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
            string id = _segment.SessionFactory.GetClassMetadata(mappingType).IdentifierPropertyName;

            IList result = idFounderAttribute.GetObject(id, context, false, _segment.CurrentSession);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        /// Find Session Attribute in the Action or Controller.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        private SessionSegment Initilize(ControllerContext controllerContext)
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
            SessionSegment segment = null;
            if (customAttributeSet.Any(customAttributes => TryEnableSession(customAttributes, out segment)))
            {
                return segment;
            }

            throw new NHModelBinderException(
                "can't find any enabled SessionAttribute on controller or action ,please special session attribute and make sure it's enabled.");
        }

        private bool TryEnableSession(object[] customAttributes, out SessionSegment segment)
        {
            segment = null;
            if (customAttributes.Length != 0)
            {
                var custommAttr = (SessionAttribute) customAttributes[0];
                if (custommAttr.Enable)
                {
                    segment = SessionManager.GetSessionFactory(custommAttr.SessionFactoryName);
                    segment.InitSession();
                    return true;
                }
            }
            return false;
        }

        private static FounderAttribute GetEntityFounderIn(Type modelType, PropertyDescriptor propertyDescriptor)
        {
            object[] customAttributes =
                modelType.GetProperty(propertyDescriptor.Name).GetCustomAttributes(typeof (FounderAttribute), true);

            if (customAttributes.Length == 0)
            {
                return new IdFounderAttribute();
            }
            return (FounderAttribute) customAttributes[0];
        }
    }
}
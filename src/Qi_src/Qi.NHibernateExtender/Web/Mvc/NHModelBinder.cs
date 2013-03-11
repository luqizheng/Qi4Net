using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NHibernate.Metadata;
using NHibernate.Type;
using Qi.NHibernateExtender;
using Qi.Web.Mvc.Founders;

namespace Qi.Web.Mvc
{
    public class NHModelBindingContext : ModelBindingContext
    {
        public ModelBindingContext Context { get; set; }

        public NHModelBindingContext(ModelBindingContext context)
        {
            Context = context;
        }

        public SessionWrapper Wrapper { get; set; }


    }
    /// <summary>
    ///     Nhibernate model biner
    /// </summary>
    public class NHModelBinder : DefaultModelBinder
    {
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
            object result = base.BindModel(controllerContext, context == null ? bindingContext : context.Context);
            return result;
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
            object result = !IsMappingClass(modelType)
                                ? base.CreateModel(controllerContext, bindingContext, modelType)
                                : GetObjectById(modelType, bindingContext);

            if (result == null)
            {
                ConstructorInfo constructor = modelType
                    .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                    null, new Type[0], new ParameterModifier[0]);
                return constructor.Invoke(null);
            }
            return result;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }

        protected override object GetPropertyValue(ControllerContext controllerContext,
                                                   ModelBindingContext bindingContext,
                                                   PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            
            var context = new NHModelBindingContext(bindingContext)
                {
                    Wrapper = this._wrapper
                };
           
            //if (IsPersistentType(bindingContext.ModelType))
            //{
            //    IClassMetadata perisisteType = _wrapper.SessionFactory.GetClassMetadata(bindingContext.ModelType);
            //    ValueProviderResult v = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            //    var id = bindingContext.ModelName + "." + perisisteType.IdentifierPropertyName;
            //    if (!bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName + "."))
            //    {
            //        //Employee should be a Employee.Id and something like that
            //        //controllerContext.HttpContext.Items.Add(id,v.RawValue);
            //        controllerContext.HttpContext.Items.Add(id, ((string[])v.RawValue)[0]);
            //    }
            //}


            object value = propertyBinder.BindModel(controllerContext, context);

            if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && Equals(value, String.Empty))
            {
                return null;
            }

            return value;

        }
        private void BindPersistentClass(ControllerContext controllerContext,
                                         ModelBindingContext bindingContext,
                                         PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            
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
            PrimitiveType identity = perisisteType.IdentifierType as PrimitiveType;
            if (identity == null)
            {
                throw new NHModelBinderException("NHModelBinder only support Id is valueType.");
            }
            string idKey = CreateSubPropertyName(bindingContext.ModelName, perisisteType.IdentifierPropertyName); //for to Role[0].Id
            var postIdValue = bindingContext.ValueProvider.GetValue(idKey).ConvertTo(identity.DefaultValue.GetType());

            if (postIdValue.Equals(identity.DefaultValue))
            {
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

            throw new NHModelBinderException(
                "can't find any enabled SessionAttribute on controller or action ,please special session attribute and make sure it's enabled.");
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
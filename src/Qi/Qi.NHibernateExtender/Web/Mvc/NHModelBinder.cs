using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NHibernate.Cfg;
using NHibernate.Mapping;
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
            customAttributes = controllerContext.Controller.GetType().GetCustomAttributes(typeof(SessionAttribute), true);
            if (customAttributes.Cast<SessionAttribute>().Any(s => s.Enable))
            {
                return;
            }

            if (customAttributes.Length == 0)
                throw new NHModelBinderException("can't find any enabled SessionAttribute on controller or action ,please special session attribute and make sure it's enabled.");

        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                             PropertyDescriptor propertyDescriptor)
        {
            ModelMetadata propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            Type parameterType;

            if (_isDto && CollectionHelper.IsCollectionType(propertyMetadata.ModelType, out parameterType))
            {
                //只有dto才能set list，如果是Domainobject，那么会忽略这个list set
                var cfg = NhConfigManager.GetNhConfig(SessionManager.CurrentSessionFactoryKey).NHConfiguration;
                PersistentClass entityTyepMapping = cfg.GetClassMapping(parameterType);
                if(entityTyepMapping==null)
                    throw new ArgumentException("can't find the mapping type "+parameterType.FullName+ " in "+ SessionManager.CurrentSessionFactoryKey);
                object[] aryIds =
                    NHMappingHelper.ConvertStringToObjects(
                        controllerContext.HttpContext.Request[propertyDescriptor.Name],
                        entityTyepMapping.Identifier.Type);

                CollectionOperator setHelper = CollectionHelper.GetSetHelper(propertyMetadata.ModelType);
                object children = setHelper.Make(propertyMetadata.ModelType, parameterType);
                foreach (object id in aryIds)
                {
                    object entity = SessionManager.Instance.GetCurrentSession().Load(parameterType, id);
                    setHelper.Add(children, entity);
                }
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, children);
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }

        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                            PropertyDescriptor propertyDescriptor, object value)
        {
            ModelMetadata propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            if (!IsPersistentType(propertyMetadata.ModelType))
            {
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
            }
            else
            {
                // property is the persistent object,
                //var s = value = "";
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
        }

        private object GetPersistentObject(ControllerContext controllerContext, PropertyDescriptor propertyDescriptor,
                                           Type modelType)
        {
            string strValue = controllerContext.RequestContext.HttpContext.Request[propertyDescriptor.Name];
            FounderAttribute founderAttribute = GetEntityFounder(propertyDescriptor, modelType);
            //SessionManager sessionManager = BuildSessionManager(propertyDescriptor.PropertyType);
            if (founderAttribute.EntityType == null)
            {
                founderAttribute.EntityType = propertyDescriptor.PropertyType;
            }
            return founderAttribute.GetObject(strValue, propertyDescriptor.Name, controllerContext.HttpContext);
        }

        public static bool IsPersistentType(Type modelType)
        {

            Configuration nhConfig =
                NhConfigManager.GetNhConfig(SessionManager.CurrentSessionFactoryKey).NHConfiguration;

            if (modelType.IsArray)
            {
                bool result = nhConfig.GetClassMapping(modelType.GetElementType()) != null;
                if (!result)
                {
                    if (modelType.GetGenericArguments().Any(type => nhConfig.GetClassMapping(type) != null))
                    {
                        return true;
                    }
                }
                return result;
            }
            if (modelType.IsGenericType)
            {
                return modelType.GetGenericArguments().Any(type => nhConfig.GetClassMapping(type) != null);
            }
            return nhConfig.GetClassMapping(modelType) != null;
        }


        private static FounderAttribute GetEntityFounder(PropertyDescriptor propertyDescriptor, Type modelType)
        {
            object[] customAttributes =
                modelType.GetProperty(propertyDescriptor.Name).GetCustomAttributes(typeof(FounderAttribute),
                                                                                   true);
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

            var nhConfig = NhConfigManager.GetNhConfig(SessionManager.CurrentSessionFactoryKey);
            PersistentClass mappingInfo = nhConfig.NHConfiguration.GetClassMapping(modelType);
            string idValue = request[mappingInfo.IdentifierProperty.Name];
            var s = new IdFounderAttribute
                        {
                            EntityType = modelType
                        };
            return s.GetObject(idValue, mappingInfo.IdentifierProperty.Name, context.HttpContext);
        }

        //private SessionManager BuildSessionManager(Type modelType)
        //{

        //    if (sessionManager.IniSession())
        //    {
        //        if (!_sessionHandlerByFilters.ContainsKey(sessionManager.Config.SessionFactoryName))
        //        {
        //            //if can't find the session in the Mvc filter, means it need to closed by this instance,
        //            _sessionHandlerByFilters.Add(sessionManager.Config.SessionFactoryName, false);
        //        }
        //        else
        //        {
        //            _sessionHandlerByFilters[sessionManager.Config.SessionFactoryName] = true;
        //        }
        //    }
        //    return sessionManager;
        //}
    }

    public class NHModelBinderException : ApplicationException
    {
        public NHModelBinderException(string message)
            : base(message)
        {

        }
    }
}
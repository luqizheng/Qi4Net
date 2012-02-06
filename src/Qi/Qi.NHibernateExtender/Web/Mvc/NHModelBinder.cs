using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NHibernate.Mapping;
using NHibernate.Type;
using Qi.Nhibernates;
using Qi.Web.Mvc.Founders;

namespace Qi.Web.Mvc
{
    public class NHModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// ModelBinder runs earlier than SessionAttribute, so this instance have to open Session and not depend SessionAttribute.
        /// We have two choice to close session, closed by SessionAttribute or closed by itself.
        /// this variable is indeciate sesson closed by SessionAttribute or itself.
        /// Key is session factory Name, value, ture means handlder by sessionAttribute, false closed by it self.
        /// </summary>
        private readonly IDictionary<string, bool> _sessionHandlerByFilters = new Dictionary<string, bool>();

        private void ClearUp()
        {
            foreach (string a in _sessionHandlerByFilters.Keys)
            {
                if (!_sessionHandlerByFilters[a]) //if value is false, cloed by this instanced.
                {
                    SessionManager.Instance.CleanUp();
                }
            }
        }

        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            base.OnModelUpdated(controllerContext, bindingContext);
            ClearUp();
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            FindAttribute(controllerContext, bindingContext);


            if (!IsPersistentType(modelType))
            {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
            HttpRequestBase request = controllerContext.RequestContext.HttpContext.Request;
            object result = GeModelFromNH(modelType, request, controllerContext);
            if (result == null)
            {
                var constructor = modelType
                       .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                       null, new Type[0], new ParameterModifier[0]);
                return constructor.Invoke(null);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        private void FindAttribute(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var reflectedControllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());

            //find on Action, sessionAttribute 's priority on action is heiher than on controller.
            string actionname = controllerContext.RouteData.Values["Action"].ToString();
            ActionDescriptor action = reflectedControllerDescriptor.FindAction(controllerContext, actionname);
            object[] at = action.GetCustomAttributes(typeof(SessionAttribute), true);
            foreach (SessionAttribute a in at)
            {
                _sessionHandlerByFilters.Add(a.SessionFactoryName, a.Enable);
            }
            at = controllerContext.Controller.GetType().GetCustomAttributes(typeof(SessionAttribute), true);
            foreach (SessionAttribute a in at)
            {
                if (!_sessionHandlerByFilters.ContainsKey(a.SessionFactoryName))
                {
                    _sessionHandlerByFilters.Add(a.SessionFactoryName, a.Enable);
                }
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
            SessionManager sessionManager = BuildSessionManager(propertyDescriptor.PropertyType);
            if (founderAttribute.EntityType == null)
            {
                founderAttribute.EntityType = propertyDescriptor.PropertyType;
            }
            return founderAttribute.GetObject(sessionManager, strValue, propertyDescriptor.Name,
                                              controllerContext.HttpContext);
        }

        private static bool IsPersistentType(Type modelType)
        {
            return SessionManager.Instance.Config.NHConfiguration.GetClassMapping(modelType) != null;
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
            SessionManager sessionManager = BuildSessionManager(modelType);
            PersistentClass mappingInfo = sessionManager.Config.NHConfiguration.GetClassMapping(modelType);
            string idValue = request[mappingInfo.IdentifierProperty.Name];
            var s = new IdFounderAttribute
                        {
                            EntityType = modelType
                        };
            return s.GetObject(sessionManager, idValue, mappingInfo.IdentifierProperty.Name, context.HttpContext);
        }

        private SessionManager BuildSessionManager(Type modelType)
        {
            SessionManager sessionManager = SessionManager.GetInstance(modelType);
            if (sessionManager.IniSession())
            {
                if (!_sessionHandlerByFilters.ContainsKey(sessionManager.Config.SessionFactoryName))
                {
                    //if can't find the session in the Mvc filter, means it need to closed by this instance,
                    _sessionHandlerByFilters.Add(sessionManager.Config.SessionFactoryName, false);
                }
                else
                {
                    _sessionHandlerByFilters[sessionManager.Config.SessionFactoryName] = true;
                }
            }
            return sessionManager;
        }
    }
}
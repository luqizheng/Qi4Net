using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NHibernate.Mapping;
using Qi.Nhibernates;

namespace Qi.Web.Mvc
{
    public class NHModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// ModelBinder runs earlier than SessionAttribute,so this instance need open session by itself.
        /// but close have two choice, closed by SessionAttribute or closed by itself.
        /// this varialbe is indeciate sesson closed by SessionAttribute or itself.
        /// 
        /// Key is session Name, value, ture means handlder by sessionAttribute, false,need close by Binder self
        /// </summary>
        private readonly IDictionary<string, bool> _sessionHandlerByFilters = new Dictionary<string, bool>();

        #region IDisposable Members

        private void ClearUp()
        {
            foreach (string a in _sessionHandlerByFilters.Keys)
            {
                if (!_sessionHandlerByFilters[a])//if value is false, cloed by this instanced.
                {
                    SessionManager.Instance.CleanUp();
                }
            }
        }

        #endregion
        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            base.OnModelUpdated(controllerContext, bindingContext);
            ClearUp();
        }
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            IndecalteAttribute(controllerContext, bindingContext);


            if (!IsPersistentType(modelType))
            {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
            HttpRequestBase request = controllerContext.RequestContext.HttpContext.Request;
            object result = GeModelFromNH(modelType, request, controllerContext);
            if (result == null)
            {
                return
                    modelType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                             null,
                                             null, null);
            }
            return result;
        }

        private void IndecalteAttribute(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            var reflectedControllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());

            string actionname = controllerContext.RouteData.Values["Action"].ToString();
            ActionDescriptor action = reflectedControllerDescriptor.FindAction(controllerContext, actionname);
            object[] at = action.GetCustomAttributes(typeof(SessionAttribute), true);
            foreach (SessionAttribute a in at)
            {
                _sessionHandlerByFilters.Add(a.SessionFactoryName, true);
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

        private object GetPersistentObject(ControllerContext controllerContext, PropertyDescriptor propertyDescriptor, Type modelType)
        {
            string strValue = controllerContext.RequestContext.HttpContext.Request[propertyDescriptor.Name];
            NhModelFounderAttribute founderAttribute = GetEntityFounder(propertyDescriptor, modelType);
            var sessionManager = BuildSessionManager(propertyDescriptor.PropertyType);
            return founderAttribute.Find(sessionManager, propertyDescriptor.Name, strValue, propertyDescriptor.PropertyType, controllerContext);
        }

        private static bool IsPersistentType(Type modelType)
        {
            return SessionManager.Instance.Config.NHConfiguration.GetClassMapping(modelType) != null;
        }

        private static NhModelFounderAttribute GetEntityFounder(PropertyDescriptor propertyDescriptor, Type modelType)
        {
            object[] customAttributes =
                modelType.GetProperty(propertyDescriptor.Name).GetCustomAttributes(typeof(NhModelFounderAttribute),
                                                                                   true);
            if (customAttributes.Length == 0)
            {
                return new NhModelFounderAttribute();
            }
            return (NhModelFounderAttribute)customAttributes[0];
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
            var s = new NhModelFounderAttribute();
            return s.Find(SessionManager.Instance, mappingInfo.IdentifierProperty.Name, idValue, modelType, context);
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
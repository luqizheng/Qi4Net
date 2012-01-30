using System;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NHibernate.Mapping;
using Qi.Nhibernates;

namespace Qi.Web.Mvc
{
    public class NHModelBinder : DefaultModelBinder, IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            SessionManager.Instance.CleanUp();
        }

        #endregion

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            if (!IsPersistentType(modelType))
            {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
            HttpRequestBase request = controllerContext.RequestContext.HttpContext.Request;
            object result = GeModelFromNH(modelType, request);
            if (result == null)
            {
                return
                    modelType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                             null,
                                             null, null);
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
            NhModelFounderAttribute founderAttribute = GetEntityFounder(propertyDescriptor, modelType);
            object value = founderAttribute.Find(SessionManager.Instance, strValue, propertyDescriptor.PropertyType);
            return value;
        }

        private static bool IsPersistentType(Type modelType)
        {
            PersistentClass res = SessionManager.Instance.Config.NHConfiguration.GetClassMapping(modelType);
            return res != null;
        }

        private static NhModelFounderAttribute GetEntityFounder(PropertyDescriptor propertyDescriptor, Type modelType)
        {
            object[] customAttributes =
                modelType.GetProperty(propertyDescriptor.Name).GetCustomAttributes(typeof (NhModelFounderAttribute),
                                                                                   true);
            if (customAttributes.Length == 0)
            {
                return new NhModelFounderAttribute();
            }
            return (NhModelFounderAttribute) customAttributes[0];
        }

        /// <summary>
        /// if model is nh mapping class, use this found to getit.
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual Object GeModelFromNH(Type modelType, HttpRequestBase request)
        {
            PersistentClass mappingInfo = SessionManager.Instance.Config.NHConfiguration.GetClassMapping(modelType);
            string idValue = request[mappingInfo.IdentifierProperty.Name];
            var s = new NhModelFounderAttribute();
            return s.Find(SessionManager.Instance, idValue, modelType);
        }
    }
}
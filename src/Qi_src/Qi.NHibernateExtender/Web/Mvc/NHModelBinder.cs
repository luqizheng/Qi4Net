using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using NHibernate;
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
        /// <param name="controllerContext">
        ///     The context within which the controller operates. The context information includes the
        ///     controller, HTTP content, request context, and route data.
        /// </param>
        /// <param name="bindingContext">
        ///     The context within which the model is bound. The context includes information such as the
        ///     model object, model name, model type, property filter, and value provider.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="bindingContext " />parameter is null.
        /// </exception>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var context = bindingContext as NHModelBindingContext;
            ModelBindingContext realContext = context == null ? bindingContext : context.Context;
            if (TypeUtility.IsBasicType(realContext.ModelType))
            {
                return base.BindModel(controllerContext, realContext);
            }

            _wrapper = context == null ? Initilize(controllerContext) : context.Wrapper;

            if (_wrapper == null)
            {
                object result = base.BindModel(controllerContext, bindingContext);
                return result;
            }

            bool addSuccess = SetWrapper(controllerContext, _wrapper);
            try
            {
            //    if (realContext.ModelType.IsArray)
            //    {
            //        return BindComplexModel(controllerContext, realContext);

            //    }
            //    else
            //    {
                    return base.BindModel(controllerContext, realContext);
              //  }
            }
            finally
            {
                if (addSuccess)
                {
                    RemoveWrapper(controllerContext);
                }
            }
        }

        private static IEnumerable<string> GetZeroBasedIndexes()
        {
            int i = 0;
            while (true)
            {
                yield return i.ToString(CultureInfo.InvariantCulture);
                i++;
            }
        }

        private static string GetUserResourceString(ControllerContext controllerContext, string resourceName)
        {
            string result = null;

            if (!String.IsNullOrEmpty(ResourceClassKey) && (controllerContext != null) &&
                (controllerContext.HttpContext != null))
            {
                result =
                    controllerContext.HttpContext.GetGlobalResourceObject(ResourceClassKey, resourceName,
                        CultureInfo.CurrentUICulture) as string;
            }

            return result;
        }

        internal object UpdateCollection(ControllerContext controllerContext, ModelBindingContext bindingContext,
            Type elementType)
        {
            bool stopOnIndexNotFound;
            IEnumerable<string> indexes;
            GetIndexes(bindingContext, out stopOnIndexNotFound, out indexes);
            IModelBinder elementBinder = Binders.GetBinder(elementType);

            // build up a list of items from the request
            var modelList = new List<object>();
            foreach (string currentIndex in indexes)
            {
                string subIndexKey = CreateSubIndexName(bindingContext.ModelName, currentIndex);
                if (!bindingContext.ValueProvider.ContainsPrefix(subIndexKey))
                {
                    if (stopOnIndexNotFound)
                    {
                        // we ran out of elements to pull
                        break;
                    }
                    continue;
                }

                var innerContext = new ModelBindingContext
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, elementType),
                    ModelName = subIndexKey,
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                object thisElement = elementBinder.BindModel(controllerContext, innerContext);

                // we need to merge model errors up
                AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, subIndexKey,
                    elementType, thisElement);
                modelList.Add(thisElement);
            }

            // if there weren't any elements at all in the request, just return
            if (modelList.Count == 0)
            {
                return null;
            }

            // replace the original collection
            object collection = bindingContext.Model;
            CollectionHelpers.ReplaceCollection(elementType, collection, modelList);
            return collection;
        }
        private static string GetValueRequiredResource(ControllerContext controllerContext)
        {
            return GetUserResourceString(controllerContext, "PropertyValueRequired") ?? "A value is required.";
        }
        private static void AddValueRequiredMessageToModelState(ControllerContext controllerContext,
            ModelStateDictionary modelState, string modelStateKey, Type elementType, object value)
        {
            if (value == null && !TypeHelpers.TypeAllowsNullValue(elementType) && modelState.IsValidField(modelStateKey))
            {
                modelState.AddModelError(modelStateKey, GetValueRequiredResource(controllerContext));
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo",
            MessageId = "System.Web.Mvc.ValueProviderResult.ConvertTo(System.Type)",
            Justification = "ValueProviderResult already handles culture conversion appropriately.")]
        private static void GetIndexes(ModelBindingContext bindingContext, out bool stopOnIndexNotFound,
            out IEnumerable<string> indexes)
        {
            string indexKey = CreateSubPropertyName(bindingContext.ModelName, "index");
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(indexKey);

            if (valueProviderResult != null)
            {
                var indexesArray = valueProviderResult.ConvertTo(typeof(string[])) as string[];
                if (indexesArray != null)
                {
                    stopOnIndexNotFound = false;
                    indexes = indexesArray;
                    return;
                }
            }

            // just use a simple zero-based system
            stopOnIndexNotFound = true;
            indexes = GetZeroBasedIndexes();
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
        ///     Remvoe NH Session, but it nevery close the session.
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
        /// <param name="controllerContext">
        ///     The context within which the controller operates. The context information includes the
        ///     controller, HTTP content, request context, and route data.
        /// </param>
        /// <param name="bindingContext">
        ///     The context within which the model is bound. The context includes information such as the
        ///     model object, model name, model type, property filter, and value provider.
        /// </param>
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
        ///     Returns the value of a property using the specified controller context, binding context, property descriptor, and
        ///     property binder.
        /// </summary>
        /// <returns>
        ///     An object that represents the property value.
        /// </returns>
        /// <param name="controllerContext">
        ///     The context within which the controller operates. The context information includes the
        ///     controller, HTTP content, request context, and route data.
        /// </param>
        /// <param name="bindingContext">
        ///     The context within which the model is bound. The context includes information such as the
        ///     model object, model name, model type, property filter, and value provider.
        /// </param>
        /// <param name="propertyDescriptor">
        ///     The descriptor for the property to access. The descriptor provides information such as
        ///     the component type, property type, and property value. It also provides methods to get or set the property value.
        /// </param>
        /// <param name="propertyBinder">An object that provides a way to bind the property.</param>
        protected override object GetPropertyValue(ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            if (!IsPersistentType(bindingContext.ModelType))
            {
                return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
            }
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
            ISessionFactory sessionFactory = _wrapper != null
                ? _wrapper.SessionFactory
                : SessionManager.Factories[SessionManager.DefaultSessionFactoryKey]
                    .SessionFactory;
            return
                types.Any(
                    type => sessionFactory.Statistics.EntityNames.Contains(type.UnderlyingSystemType.FullName));
        }

        /// <summary>
        ///     return value  this modelType is belong to Mappling class or not.
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private bool IsMappingClass(Type modelType)
        {
            if (_wrapper == null)
            {
                return SessionManager.Factories[SessionManager.DefaultSessionFactoryKey].SessionFactory.Statistics
                    .EntityNames
                    .Contains(
                        modelType
                            .UnderlyingSystemType
                            .FullName);
            }

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
                ValueProviderResult valurProvider = bindingContext.ValueProvider.GetValue(idKey);
                if (valurProvider == null)
                    return null;
                object postIdValue = valurProvider.ConvertTo(identity.DefaultValue.GetType());
                if (postIdValue == null || postIdValue.Equals(identity.DefaultValue))
                {
                    return CreateInstanceHelper.CreateInstance(mappingType);
                    //return identity.DefaultValue;
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

        private static class CollectionHelpers
        {
            private static readonly MethodInfo _replaceCollectionMethod = typeof(CollectionHelpers).GetMethod("ReplaceCollectionImpl", BindingFlags.Static | BindingFlags.NonPublic);
            private static readonly MethodInfo _replaceDictionaryMethod = typeof(CollectionHelpers).GetMethod("ReplaceDictionaryImpl", BindingFlags.Static | BindingFlags.NonPublic);

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void ReplaceCollection(Type collectionType, object collection, object newContents)
            {
                MethodInfo targetMethod = _replaceCollectionMethod.MakeGenericMethod(collectionType);
                targetMethod.Invoke(null, new object[] { collection, newContents });
            }

            private static void ReplaceCollectionImpl<T>(ICollection<T> collection, IEnumerable newContents)
            {
                collection.Clear();
                if (newContents != null)
                {
                    foreach (object item in newContents)
                    {
                        // if the item was not a T, some conversion failed. the error message will be propagated,
                        // but in the meanwhile we need to make a placeholder element in the array.
                        T castItem = (item is T) ? (T)item : default(T);
                        collection.Add(castItem);
                    }
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void ReplaceDictionary(Type keyType, Type valueType, object dictionary, object newContents)
            {
                MethodInfo targetMethod = _replaceDictionaryMethod.MakeGenericMethod(keyType, valueType);
                targetMethod.Invoke(null, new object[] { dictionary, newContents });
            }

            private static void ReplaceDictionaryImpl<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<object, object>> newContents)
            {
                dictionary.Clear();
                foreach (KeyValuePair<object, object> item in newContents)
                {
                    if (item.Key is TKey)
                    {
                        // if the item was not a T, some conversion failed. the error message will be propagated,
                        // but in the meanwhile we need to make a placeholder element in the dictionary.
                        TKey castKey = (TKey)item.Key; // this cast shouldn't fail
                        TValue castValue = (item.Value is TValue) ? (TValue)item.Value : default(TValue);
                        dictionary[castKey] = castValue;
                    }
                }
            }
        }

        internal object BindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object model = bindingContext.Model;
            Type modelType = bindingContext.ModelType;

            // if we're being asked to create an array, create a list instead, then coerce to an array after the list is created
            if (model == null && modelType.IsArray)
            {
                Type elementType = modelType.GetElementType();
                Type listType = typeof(List<>).MakeGenericType(elementType);
                object collection = CreateModel(controllerContext, bindingContext, listType);

                ModelBindingContext arrayBindingContext = new ModelBindingContext()
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => collection, listType),
                    ModelName = bindingContext.ModelName,
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                IList list = (IList)UpdateCollection(controllerContext, arrayBindingContext, elementType);

                if (list == null)
                {
                    return null;
                }

                Array array = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(array, 0);
                return array;
            }

            if (model == null)
            {
                model = CreateModel(controllerContext, bindingContext, modelType);
            }

            // special-case IDictionary<,> and ICollection<>
            Type dictionaryType = TypeHelpers.ExtractGenericInterface(modelType, typeof(IDictionary<,>));
            if (dictionaryType != null)
            {
                /*Type[] genericArguments = dictionaryType.GetGenericArguments();
                Type keyType = genericArguments[0];
                Type valueType = genericArguments[1];

                ModelBindingContext dictionaryBindingContext = new ModelBindingContext()
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType),
                    ModelName = bindingContext.ModelName,
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                object dictionary = UpdateDictionary(controllerContext, dictionaryBindingContext, keyType, valueType);
                return dictionary;*/
            }

            Type enumerableType = TypeHelpers.ExtractGenericInterface(modelType, typeof(IEnumerable<>));
            if (enumerableType != null)
            {
                Type elementType = enumerableType.GetGenericArguments()[0];

                Type collectionType = typeof(ICollection<>).MakeGenericType(elementType);
                if (collectionType.IsInstanceOfType(model))
                {
                    ModelBindingContext collectionBindingContext = new ModelBindingContext()
                    {
                        ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType),
                        ModelName = bindingContext.ModelName,
                        ModelState = bindingContext.ModelState,
                        PropertyFilter = bindingContext.PropertyFilter,
                        ValueProvider = bindingContext.ValueProvider
                    };
                    object collection = UpdateCollection(controllerContext, collectionBindingContext, elementType);
                    return collection;
                }
            }

            // otherwise, just update the properties on the complex type
            var i =0;
            //BindComplexElementalModel(controllerContext, bindingContext, model);
            return model;
        }
    }
}
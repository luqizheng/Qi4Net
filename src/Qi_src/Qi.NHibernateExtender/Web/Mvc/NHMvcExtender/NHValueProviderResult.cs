using System;
using System.Globalization;
using System.Web.Mvc;
using NHibernate.Metadata;
using Qi.NHibernateExtender;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// </summary>
    public class NHValueProviderResult : ValueProviderResult
    {
        private readonly SessionWrapper _sessionWrapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="sessionWrapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public NHValueProviderResult(ValueProviderResult result, SessionWrapper sessionWrapper)
        {
            _sessionWrapper = sessionWrapper;
            AttemptedValue = result.AttemptedValue;
            Culture = result.Culture;
            RawValue = result.RawValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object ConvertTo(Type type, CultureInfo culture)
        {
            if (type.IsValueType || typeof(string) == type)
                return base.ConvertTo(type, culture);
            Type elementType = type;
            if (type.IsArray)
            {
                elementType = type.GetElementType();
            }
            IClassMetadata perisisteType = SessionManager.GetSessionWrapper().SessionFactory.GetClassMetadata(elementType);
            if (perisisteType == null)
                return base.ConvertTo(elementType, culture);
            else if (type.IsArray)
            {
                return GetArray(elementType, perisisteType, culture);
            }
            object id = base.ConvertTo(perisisteType.IdentifierType.ReturnedClass, culture);
            return _sessionWrapper.CurrentSession.Get(elementType, id);
        }

        private object GetArray(Type elemetType, IClassMetadata classMetadata, CultureInfo culture)
        {
            var idArray = (object[])base.ConvertTo(Array.CreateInstance(classMetadata.IdentifierType.ReturnedClass, 0).GetType(),
                                         culture);
            Array result = Array.CreateInstance(elemetType, idArray.Length);
            int i = 0;
            foreach (var id in idArray)
            {
                if (!String.IsNullOrEmpty(id.ToString()))
                {
                    var pesisite = _sessionWrapper.CurrentSession.Get(elemetType, id);
                    result.SetValue(pesisite, i);
                    i++;
                }
            }
            return result;
        }
    }
}
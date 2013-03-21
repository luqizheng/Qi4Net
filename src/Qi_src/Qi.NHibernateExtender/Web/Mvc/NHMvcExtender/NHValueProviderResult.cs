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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="sessionWrapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public NHValueProviderResult(ValueProviderResult result, SessionWrapper sessionWrapper)
        {
            if (result == null)
                throw new ArgumentNullException("result");

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
            IClassMetadata perisisteType =
                SessionManager.GetSessionWrapper().SessionFactory.GetClassMetadata(type);
            if (perisisteType == null)
                return base.ConvertTo(type, culture);
            object id = base.ConvertTo(perisisteType.IdentifierType.ReturnedClass, culture);
            return SessionManager.GetSessionWrapper()
                                 .CurrentSession.Get(type, id);
        }
    }
}
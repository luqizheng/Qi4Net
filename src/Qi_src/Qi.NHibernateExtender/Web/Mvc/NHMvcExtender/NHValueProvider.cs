using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;
using Qi.NHibernateExtender;

namespace Qi.Web.Mvc.NHMvcExtender
{
    /// <summary>
    /// </summary>
    public class NHValueProvider : NameValueCollectionValueProvider
    {
        private readonly ControllerContext _controllerContext;

        /// <summary>
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="collection"></param>
        public NHValueProvider(ControllerContext controllerContext, NameValueCollection collection)
            : base(collection, CultureInfo.CurrentCulture)
        {
            _controllerContext = controllerContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override ValueProviderResult GetValue(string key)
       {
           ValueProviderResult result = null;
           if (!key.EndsWith("]"))
           {
               result = base.GetValue(key);
           }
           else
           {
               var key1 = RemoveArraySufix(key);
               var keyset = new String[]
               {
                   key1 + "[]",
                   key,
                   key1,
               };
               foreach (var aKey in keyset)
               {
                   result = base.GetValue(aKey);
                   if (result != null)
                       break;
               }
           }
           if (result == null)
                return null;
            return new NHValueProviderResult(result,
                                             NHModelBinder.GetWrapper(_controllerContext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="skipValidation"></param>
        /// <returns></returns>
        public override ValueProviderResult GetValue(string key, bool skipValidation)
        {
            ValueProviderResult result = null;
            if (!key.EndsWith("]"))
            {
                result = base.GetValue(key,skipValidation);
            }
            else
            {
                var key1 = RemoveArraySufix(key);
                var keyset = new String[]
               {
                   key1 + "[]",
                   key,
                   key1,
               };
                foreach (var aKey in keyset)
                {
                    result = base.GetValue(aKey,skipValidation);
                    if (result != null)
                        break;
                }
            }
            if (result == null)
                return null;
            return new NHValueProviderResult(result, NHModelBinder.GetWrapper(_controllerContext));
        }

        private string RemoveArraySufix(string key)
        {
            var sta = key.LastIndexOf('[');
            return key.Substring(0, sta);

        }


    }
}
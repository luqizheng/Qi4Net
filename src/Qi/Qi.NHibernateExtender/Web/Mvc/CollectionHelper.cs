using System;
using System.Collections.Generic;
using System.Linq;

namespace Qi.Web.Mvc
{
    public static class CollectionHelper
    {
        private static readonly Type[] CollectionSet = new[]
                                                           {
                                                               typeof (Iesi.Collections.Generic.ISet<>),
                                                               typeof (IList<>),
                                                               typeof (ISet<>),
                                                               typeof (ICollection<>),
                                                           };


        public static bool IsCollectionType(Type modelType, out Type parameterType)
        {
            parameterType = null;
            if (modelType.IsArray)
            {
                parameterType = modelType.GetElementType();
                return true;
            }
            if (modelType.IsGenericType)
            {
                Type unboundtype = modelType.GetGenericTypeDefinition();

                bool result = CollectionSet.Any(type => type == unboundtype);
                if (result)
                {
                    parameterType = modelType.GetGenericArguments()[0];
                }
                return result;
            }
            return false;
        }
    }
}
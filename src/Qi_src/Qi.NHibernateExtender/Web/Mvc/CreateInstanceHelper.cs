using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Qi.Web.Mvc
{
    internal static class CreateInstanceHelper
    {
        public static object CreateInstance(Type modelType)
        {
            ConstructorInfo constructor = modelType
                    .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                    null, new Type[0], new ParameterModifier[0]);
            return constructor.Invoke(null);
        }
    }
}

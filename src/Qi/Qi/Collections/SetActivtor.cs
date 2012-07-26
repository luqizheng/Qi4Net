using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qi.Collections
{
    /// <summary>
    /// Setter for Collections.
    /// </summary>
    public class SetActivtor
    {
        private static readonly Dictionary<Type, Type> DefaultSetType = new Dictionary<Type, Type>
                                                                            {
                                                                                {typeof(Iesi.Collections.Generic.ISet<>),typeof(Iesi.Collections.Generic.HashedSet<>)},
                                                                                {typeof (ISet<>), typeof (HashSet<>)},
                                                                                {typeof (IList<>), typeof (List<>)},
                                                                                {typeof (IList), typeof (ArrayList)},
                                                                                {typeof (ArrayList), typeof (ArrayList)},
                                                                                {
                                                                                    typeof (IEnumerable<>), typeof (List<>)
                                                                                    },
                                                                            };

        private readonly Type _instanceType;

        public SetActivtor(Type instanceType)
        {
            if (instanceType == null)
                throw new ArgumentNullException("instanceType");
            _instanceType = instanceType;
        }

        /// <summary>
        /// 1 is the instance type, 2st is the child element type,
        /// 3st int32 is the capacity,
        /// </summary>
        public Func<Type, Type, int, object> ActiveMethod { get; set; }

        public object Create(Type childElementType, int capacity)
        {
            return ActiveMethod(_instanceType, childElementType, capacity);
        }

        public SetAccessor CreateAccessor(object target)
        {
            return SetAccessor.Create(target);
        }

        public static bool IsSupport(Type modelType, out Type parameterType)
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

                bool result = DefaultSetType.Keys.Any(type => type == unboundtype);
                if (result)
                {
                    parameterType = modelType.GetGenericArguments()[0];
                }
                return result;
            }
            return false;
        }

        public static SetActivtor Create(Type instanceType)
        {
            Type targetType = instanceType;
            Func<Type, Type, int, object> activeMethod = null;

            if (instanceType.IsArray)
            {
                activeMethod = (t1, t2, t3) => Array.CreateInstance(t1.GetElementType(), t3);
            }

            if (instanceType.IsInterface)
            {
                Type genericType = instanceType.GetGenericTypeDefinition();
                if (!DefaultSetType.ContainsKey(genericType))
                    throw new ArgumentException("It doesn't support for genericType" + genericType.Name);

                targetType = DefaultSetType[genericType];
                activeMethod = (t1, t2, t3) => Activator.CreateInstance(t1.MakeGenericType(t2));
            }

            return new SetActivtor(targetType)
                       {
                           ActiveMethod = activeMethod ?? ((t1, t2, t3) => Activator.CreateInstance(instanceType))
                       };
        }
    }
}
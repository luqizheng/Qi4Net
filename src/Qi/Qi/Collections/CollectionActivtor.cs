using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qi.Collections
{
    /// <summary>
    /// Create instance base on colection type.
    /// </summary>
    public class CollectionActivtor
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceType"></param>
        public CollectionActivtor(Type instanceType)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="childElementType"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public object Create(Type childElementType, int capacity)
        {
            return ActiveMethod(_instanceType, childElementType, capacity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CollectionAccessor CreateAccessor(object target)
        {
            return CollectionAccessor.Create(target);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="parameterType"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public static CollectionActivtor Create(Type instanceType)
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

            return new CollectionActivtor(targetType)
                       {
                           ActiveMethod = activeMethod ?? ((t1, t2, t3) => Activator.CreateInstance(instanceType))
                       };
        }
    }
}
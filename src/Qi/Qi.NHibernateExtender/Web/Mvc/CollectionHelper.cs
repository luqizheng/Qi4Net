using System;
using System.Collections.Generic;
using System.Linq;

namespace Qi.Web.Mvc
{
    public static class CollectionHelper
    {
        //    private static readonly Dictionary<Type, Func<Type, Type, object>> Maker;
        //    private static readonly Dictionary<Type, VoidFunc<object, object>> Adders;

        private static readonly Type[] CollectionSet = new[]
                                                           {
                                                               typeof (Iesi.Collections.Generic.ISet<>),
                                                               typeof (IList<>),
                                                               typeof (ISet<>),
                                                               typeof (ICollection<>),
                                                           };

        //    static CollectionHelper()
        //    {
        //        ////优先接口，后实现
        //        Maker = new Dictionary<Type, Func<Type, Type, object>>
        //                    {
        //                        {typeof (Iesi.Collections.Generic.ISet<>), CollectionOperator.DefaultSetMaker},
        //                        {typeof (IList<>), CollectionOperator.DefaultListMaker},
        //                        {typeof (ICollection<>), CollectionOperator.DefaultListMaker},
        //                        {typeof (IList), CollectionOperator.DefaultListMaker}
        //                    };
        //        Adders = new Dictionary<Type, VoidFunc<object, object>>
        //                     {
        //                         {typeof (Iesi.Collections.Generic.ISet<>), CollectionOperator.SetAdd},
        //                         {typeof (IList<>), CollectionOperator.ListAdd},
        //                         {typeof (ICollection<>), CollectionOperator.ListAdd},
        //                         {typeof (IList), CollectionOperator.ListAdd}
        //                     };
        //    }


        //    public static CollectionOperator GetSetHelper(Type collectionType)
        //    {
        //        bool useDefaultCollection = collectionType.IsInterface;

        //        Type defaultSetting = useDefaultCollection ? collectionType.GetGenericTypeDefinition() : collectionType;
        //        if (Maker.ContainsKey(defaultSetting))
        //        {
        //            return new CollectionOperator
        //                       {
        //                           Add = Adders[defaultSetting],
        //                           Make = Maker[defaultSetting]
        //                       };
        //        }

        //        Type[] interfaceSet = collectionType.GetInterfaces().Where(t => t.IsGenericType).ToArray();


        //        foreach (Type interfaceType in interfaceSet)
        //        {
        //            Type intType = interfaceType.GetGenericTypeDefinition();

        //            if (intType == typeof (ICollection<>) || intType == typeof (IList<>))
        //            {
        //                return new CollectionOperator
        //                           {
        //                               Add = CollectionOperator.ListAdd,
        //                               Make =
        //                                   useDefaultCollection
        //                                       ? CollectionOperator.DefaultSetMaker
        //                                       : CollectionOperator.GenericMaker
        //                           };
        //            }
        //            if (intType == typeof (Iesi.Collections.Generic.ISet<>))
        //            {
        //                return new CollectionOperator
        //                           {
        //                               Add = CollectionOperator.SetAdd,
        //                               Make =
        //                                   useDefaultCollection
        //                                       ? CollectionOperator.DefaultSetMaker
        //                                       : CollectionOperator.GenericMaker
        //                           };
        //            }
        //        }
        //        throw new NotImplementedException("Collection only support ISet IList ISet<T> IList<T>");
        //    }


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
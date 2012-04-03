using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections;

namespace Qi.Web.Mvc
{
    //public class CollectionOperator
    //{
    //    public static Func<Type, Type, object> Maker = (t1, t2) => Activator.CreateInstance(t1);
    //    public static Func<Type, Type, object> GenericMaker = (t1, t2) => Activator.CreateInstance(t1.MakeGenericType(t2));
    //    public static Func<Type, Type, object> DefaultSetMaker = (listType, t) => Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
    //    public static Func<Type, Type, object> DefaultListMaker = (listType, t) => Activator.CreateInstance(typeof(List<>).MakeGenericType(t));

    //    public static VoidFunc<object, object> SetAdd = (target, val) => ((ISet)target).Add(val);
    //    public static VoidFunc<object, object> ListAdd = (target, val) => ((IList)target).Add(val);

    //    /// <summary>
    //    /// 1.Type is the generic type defined, 2st type is the item's type
    //    /// </summary>
    //    public Func<Type, Type, object> Make { get; set; }

    //    public VoidFunc<object, object> Add { get; set; }
    //}
}
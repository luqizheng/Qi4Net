﻿using System;
using System.Collections;
using Iesi.Collections;

namespace Qi.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public class SetAccessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public SetAccessor(object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            Target = target;
        }
        /// <summary>
        /// 
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VoidFunc<object, object> AddMethod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VoidFunc<object, object, int> SetMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Func<object, int> CountMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public void SetOrAdd(object item, int index)
        {
            if (CountMethod(item) <= index && index == -1)
            {
                if (AddMethod == null)
                    throw new ArgumentOutOfRangeException("index", "index is larget unbound of target");
                AddMethod(Target, item);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(object item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AddMethod(Target, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public void Set(object item, int index)
        {
            SetMethod(Target, item, index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static SetAccessor Create(object target)
        {
            VoidFunc<object, object> addMethod = null;
            Func<object, int> countMethod = null;
            VoidFunc<object, object, int> setMethod = null;
            if (target is IList)
            {
                addMethod = (t1, t2) => ((IList) t1).Add(t2);
                setMethod = (t1, t2, t3) => ((IList) t1)[t3] = t2;
                countMethod = t1 => ((IList) t1).Count;
            }
            else if (target is ISet)
            {
                addMethod = (t1, t2) => ((ISet) t1).Add(t2);
                setMethod = (t1, t2, t3) => ((ISet) t1).Add(t2);
                countMethod = t1 => ((ISet) t1).Count;
            }
            else if (target.GetType().IsArray)
            {
                setMethod = (t1, t2, t3) => ((Array) t1).SetValue(t2, t3);
                countMethod = t1 => ((Array) t1).Length;
            }
            else
            {
                throw new ApplicationException("SetAccessor only support IList,Iesi.Collection.ISet and array.");
            }

            return new SetAccessor(target)
                       {
                           AddMethod = addMethod,
                           SetMethod = setMethod,
                           CountMethod = countMethod,
                       };
        }
    }
}
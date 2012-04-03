using System;
using System.Collections;
using Iesi.Collections;

namespace Qi.Collections
{
    public class SetAccessor
    {
        public SetAccessor(object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            Target = target;
        }

        public object Target { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VoidFunc<object, object> AddMethod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VoidFunc<object, object, int> SetMethod { get; set; }

        public Func<object, int> CountMethod { get; set; }

        public void SetOrAdd(object item, int index)
        {
            if (CountMethod(item) <= index && index == -1)
            {
                if (AddMethod == null)
                    throw new ArgumentOutOfRangeException("index", "index is larget unbound of target");
                AddMethod(this.Target, item);
            }


        }

        public void Add(object item)
        {
            AddMethod(Target, item);
        }

        public void Set(object item, int index)
        {
            SetMethod(Target, item, index);
        }


        public static SetAccessor Create(object target)
        {
            VoidFunc<object, object> addMethod = null;
            Func<object, int> countMethod = null;
            VoidFunc<object, object, int> setMethod = null;
            if (target is IList)
            {
                addMethod = (t1, t2) => ((IList)t1).Add(t2);
                setMethod = (t1, t2, t3) => ((IList)t1)[t3] = t2;
                countMethod = t1 => ((IList)t1).Count;
            }
            else if (target is ISet)
            {
                addMethod = (t1, t2) => ((ISet)t1).Add(t2);
                setMethod = (t1, t2, t3) => ((ISet)t1).Add(t2);
                countMethod = t1 => ((ISet)t1).Count;
            }
            else
            {
                setMethod = (t1, t2, t3) => ((Array)t1).SetValue(t2, t3);
                countMethod = t1 => ((Array)t1).Length;
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
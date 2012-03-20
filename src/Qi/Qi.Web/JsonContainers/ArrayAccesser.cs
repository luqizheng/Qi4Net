using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi.Web.JsonContainers
{
    internal class ArrayAccesser : Accesser
    {
        private readonly Array _ary;

        public ArrayAccesser(Array ary)
            : base(ary)
        {
            _ary = ary;
        }

        public override void Set(IEnumerable objects, int startIndex)
        {
            var list = new ArrayList(_ary);
            while (startIndex > list.Count)
            {
                list.Add(null);
            }
            foreach (var o in objects)
            {
                list[startIndex] = o;
                startIndex++;
            }
        }

        public override int Count
        {
            get { return _ary.Length; }
        }


    }
}

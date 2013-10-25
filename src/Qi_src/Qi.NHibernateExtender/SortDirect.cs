using System;
using System.Linq.Expressions;

namespace Qi
{
    public enum SortDirect
    {
        Asc,
        Desc
    }


    public class SortTarget
    {
        public SortDirect Tag { get; set; }
        public Expression<Func<object>> Property { get; set; }
    }
}
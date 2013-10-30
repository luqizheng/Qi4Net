using System;
using System.Linq.Expressions;

namespace Qi
{
    /// <summary>
    /// </summary>
    public enum SortDirect
    {
        /// <summary>
        /// </summary>
        Asc,

        /// <summary>
        /// </summary>
        Desc
    }

    /// <summary>
    /// </summary>
    public class SortTarget
    {
        /// <summary>
        /// </summary>
        public SortDirect Tag { get; set; }

        /// <summary>
        /// </summary>
        public Expression<Func<object>> Property { get; set; }

        /// <summary>
        /// </summary>
        public Symbol Symbol { get; set; }
    }

    /// <summary>
    /// </summary>
    public class SearchCondition
    {
        /// <summary>
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// </summary>
        public Expression<Func<object>> Property { get; set; }
    }

    /// <summary>
    /// </summary>
    public enum Symbol
    {
        /// <summary>
        /// </summary>
        Equal,

        /// <summary>
        /// </summary>
        Like,

        /// <summary>
        /// </summary>
        GreaterThan,

        /// <summary>
        /// </summary>
        GreaterEqual,

        /// <summary>
        /// </summary>
        LessThan,

        /// <summary>
        /// </summary>
        LessEqual,
    }
}
using System;

namespace Qi.DataTables.Columns
{
    /// <summary>
    /// Reference Column
    /// </summary>
    /// <typeparam name="T">object of this column for show.</typeparam>
    /// <typeparam name="TReturnValue">Column 's value type</typeparam>
    public class ReferenceColumn<T, TReturnValue> : AbstractColumn<TReturnValue>
    {
        private readonly IColumn _reference;
        private readonly Func<T, bool> _showCondition;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reference"></param>
        /// <param name="showCondition"></param>
        public ReferenceColumn(string name, IColumn reference, Func<T, bool> showCondition)
            : base(name)
        {
            _reference = reference;
            _showCondition = showCondition;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        protected override object InvokeObject(object rowObject)
        {
            if (_showCondition((T) rowObject))
            {
                return _reference.GetValue(rowObject);
            }
            return null;
        }
    }
}
using System;

namespace Qi.DataTables.Columns
{
    /// <summary>
    /// Column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TReturnValue"></typeparam>
    public class Column<T, TReturnValue> : AbstractColumn<TReturnValue>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Column(string name)
            : base(name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Func<T, TReturnValue> Accessor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        protected override object InvokeObject(object rowObject)
        {
            if (Accessor == null)
                return null;
            TReturnValue result = Accessor.Invoke((T) rowObject);
            return result;
        }
    }
}
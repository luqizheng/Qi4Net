using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Qi.DataTables
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnCollection : IEnumerable<IColumn>
    {
        private readonly Collections _columns = new Collections();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public IColumn this[string columnName]
        {
            get
            {
                if (_columns.Contains(columnName))
                {
                    return _columns[columnName];
                }
                throw new ArgumentOutOfRangeException(string.Format("can't find column named {0}.", columnName));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IColumn this[int index]
        {
            get { return _columns[index]; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _columns.Count; }
        }

        #region IEnumerable<IColumn> Members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IColumn> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _columns.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return _columns.Contains(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        public void Add(IColumn column)
        {
            if (_columns.Contains(column.Name))
            {
                throw new ArgumentException("Duplicate column's name");
            }
            _columns.Add(column);
        }

        #region Nested type: Collections

        private class Collections : KeyedCollection<string, IColumn>
        {
            protected override string GetKeyForItem(IColumn item)
            {
                return item.Name;
            }
        }

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Qi.DataTables
{
    public class ColumnCollection : IEnumerable<IColumn>
    {
        private readonly Collections _columns = new Collections();

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

        public IColumn this[int index]
        {
            get { return _columns[index]; }
        }

        public int Count
        {
            get { return _columns.Count; }
        }

        #region IEnumerable<IColumn> Members

        public IEnumerator<IColumn> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void Clear()
        {
            _columns.Clear();
        }

        public bool Contains(string key)
        {
            return _columns.Contains(key);
        }

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
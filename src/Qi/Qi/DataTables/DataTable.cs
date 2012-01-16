using System;
using System.Collections.Generic;
using System.Linq;
using Qi.DataTables.Columns;

namespace Qi.DataTables
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataTable<T> : IDataTable
    {
        private ColumnCollection _columns = new ColumnCollection();

        private IEnumerable<T> _data;

        #region IDataTable Members

        /// <summary>
        /// 
        /// </summary>
        public ColumnCollection Columns
        {
            get { return _columns ?? (_columns = new ColumnCollection()); }
        }

        /// <summary>
        /// 获取所有的Columns
        /// </summary>
        public string[] ColumnNames
        {
            get { return (from v in Columns select v.Name).ToArray(); }
        }

        /// <summary>
        /// 獲取所有的Rows
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object[]> GetRows()
        {
            if (_data == null)
            {
                throw new CreatingResultException("Please call SetData method first.");
            }
            var result = new List<object[]>();
            if (_data != null)
            {
                foreach (T data in _data)
                {
                    var item = new object[_columns.Count];
                    for (int index = 0; index < Columns.Count; index++)
                    {
                        item[index] = Columns[index].GetValue(data);
                    }
                    result.Add(item);
                }
            }
            foreach (IColumn column in Columns)
            {
                column.Clear();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        IDataTable IDataTable.SetData(IEnumerable<object> items)
        {
            List<T> a = items.Select(item => (T)item).ToList();
            return SetData(a);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasRows
        {
            get { return _data != null && _data.Any(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorName"></param>
        /// <returns></returns>
        public object[] GetSummaries(string calculatorName)
        {
            var result = new object[Columns.Count];
            int index = 0;
            foreach (IColumn column in Columns)
            {
                if (column.HasCaculator(calculatorName))
                {
                    result[index] = column.GetResult(calculatorName);
                }
                index++;
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object[] GetSummaries()
        {
            return GetSummaries(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorIndex"></param>
        /// <returns></returns>
        public object[] GetSummaries(int calculatorIndex)
        {
            var result = new object[Columns.Count];
            int index = 0;
            foreach (IColumn column in Columns)
            {
                result[index] = column.GetResult(calculatorIndex);
                index++;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturnValue"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="accessor"></param>
        /// <returns></returns>
        public IColumn Column<TReturnValue>(string columnName, Func<T, TReturnValue> accessor)
        {
            if (columnName == null)
                throw new ArgumentNullException("columnName");
            if (accessor == null)
                throw new ArgumentNullException("accessor");
            if (Columns.Contains(columnName))
            {
                throw new ArgumentOutOfRangeException(columnName + " is duplicate column name.");
            }
            var column = new Column<T, TReturnValue>(columnName) { Accessor = accessor };
            Columns.Add(column);
            return column;
        }

        /// <summary>
        /// set the data in this DataTable
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public DataTable<T> SetData(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            _data = items;
            return this;
        }
    }
}
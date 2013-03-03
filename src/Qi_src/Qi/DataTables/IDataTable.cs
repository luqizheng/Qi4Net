using System;
using System.Collections.Generic;

namespace Qi.DataTables
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataTable
    {
        /// <summary>
        /// 
        /// </summary>
        ColumnCollection Columns { get; }

        /// <summary>
        /// 
        /// </summary>
        bool HasRows { get; }

        /// <summary>
        /// 
        /// </summary>
        string[] ColumnNames { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<object[]> GetRows();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        IDataTable SetData(IEnumerable<object> items);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorName"></param>
        /// <returns></returns>
        object[] GetSummaries(string calculatorName);
    }
}
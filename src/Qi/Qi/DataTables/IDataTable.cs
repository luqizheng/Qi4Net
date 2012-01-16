using System;
using System.Collections.Generic;

namespace Qi.DataTables
{
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

        object[] GetSummaries(string calculatorName);
    }
}
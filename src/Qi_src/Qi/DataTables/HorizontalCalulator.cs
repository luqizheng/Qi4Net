using System;
using System.Linq;
using Qi.DataTables.Columns;

namespace Qi.DataTables
{
    /// <summary>
    /// calulate multi column in a row.
    /// </summary>
    public static class HorizontalCalulator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static IColumn[] GetColumns(this IDataTable table, params string[] columnNames)
        {
            return columnNames.Select(name => table.Columns[name]).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TColumnValue"></typeparam>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static IColumn Sum<TColumnValue>(this IDataTable table, string columnName,
                                                params IColumn[] columns)
        {
            var column = new CalculatorColumn<TColumnValue>(columnName,
                                                            Calculator.CreateSumCalculator(typeof (TColumnValue)),
                                                            columns);
            table.Columns.Add(column);
            return column;
        }

        /// <summary>
        /// 创建一个列，该列的结果由其他列相加而成。
        /// </summary>
        /// <typeparam name="TReturnValue"></typeparam>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static IColumn Sum<TReturnValue>(this IDataTable table, string columnName,
                                                params string[] columnNames)
        {
            IColumn[] columns = table.GetColumns(columnNames);
            var column = new CalculatorColumn<TReturnValue>(columnName,
                                                            Calculator.CreateSumCalculator(typeof (TReturnValue)),
                                                            columns);
            table.Columns.Add(column);
            return column;
        }

        /// <summary>
        /// 创建一个列，这个列的结果由其他列组合而成的。有方法计算结果
        /// </summary>
        /// <typeparam name="TReturnVaue">计算结果的类型</typeparam>
        /// <param name="table">数据</param>
        /// <param name="columnName">列的名称</param>
        /// <param name="customeFunc">计算结果列的方法</param>
        /// <param name="columnNames">这个列有哪些列计算而成</param>
        /// <returns></returns>
        public static IColumn Column<TReturnVaue>(this IDataTable table, string columnName,
                                                  Func<object[], TReturnVaue> customeFunc,
                                                  params string[] columnNames)
        {
            IColumn[] columns = table.GetColumns(columnNames);

            var column = new CustomeCalculatorColumn<TReturnVaue>(columnName, customeFunc, columns);
            table.Columns.Add(column);
            return column;
        }
    }
}
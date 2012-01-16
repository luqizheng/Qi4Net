using System;
using Qi.DataTables.Calculators;

namespace Qi.DataTables
{
    public static class SummaryHelper
    {
        public static object SumResult(this IColumn column)
        {
            if (column.HasCaculator(Sum<int>.SumCalculatorName))
            {
                return column.GetResult(Sum<int>.SumCalculatorName);
            }
            throw new ArgumentException(string.Format("Column {0} do not set the Sum function.", Sum<int>.SumCalculatorName));
        }


        public static object AvgResult(this IColumn column)
        {
            if (column.HasCaculator(Avg<int>.AvgCalculatorName))
            {
                return column.GetResult(Sum<int>.AvgCalculatorName);
            }
            throw new ArgumentException(string.Format("Column {0} do not set the Sum function.", Sum<int>.AvgCalculatorName));
        }
    }
}
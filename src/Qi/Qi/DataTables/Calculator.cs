using System;
using System.Collections.Generic;
using Qi.DataTables.Calculators;
using Qi.DataTables.Calculators.Avgs;
using Qi.DataTables.Calculators.Sums;

namespace Qi.DataTables
{
    /// <summary>
    /// include all calculator
    /// </summary>
    public static class Calculator
    {
        private static readonly Dictionary<Type, Func<ICalculator>> SumMap
            = new Dictionary<Type, Func<ICalculator>>
                  {
                      {typeof (int), SumInt32.Create},
                      {
                          typeof (int?),
                          SumInt32Nullable.Create
                          },
                      {typeof (long), SumInt64.Create},
                      {
                          typeof (long?),
                          SumInt64Nullable.Create
                          },
                      {
                          typeof (decimal),
                          SumDecimal.Create
                          },
                      {
                          typeof (decimal?),
                          SumDecimalNullable.Create
                          },
                      {typeof (Single), SumSingle.Create},
                      {
                          typeof (Single?),
                          SumSingleNullable.Create
                          },
                      {typeof (double), SumDouble.Create},
                      {
                          typeof (double?),
                          SumDoubleNullable.Create
                          },
                  };

        private static readonly Dictionary<Type, Func<ICalculator>> AvgMap
            = new Dictionary<Type, Func<ICalculator>>
                  {
                      {typeof (int), AvgInt32.Create},
                      {
                          typeof (int?),
                          AvgInt32Nullable.Create
                          },
                      {typeof (long), AvgInt64.Create},
                      {
                          typeof (long?),
                          AvgInt64Nullable.Create
                          },
                      {
                          typeof (decimal),
                          AvgDecimal.Create
                          },
                      {
                          typeof (decimal?),
                          AvgDecimalNullable.Create
                          },
                      {
                          typeof (Single), AvgDecimal.Create
                          },
                      {
                          typeof (Single?),
                          AvgDecimalNullable.Create
                          },
                      {
                          typeof (double), AvgDecimal.Create
                          },
                      {
                          typeof (double?),
                          AvgDecimalNullable.Create
                          },
                  };

        /// <summary>
        /// sum all row in this column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public static IColumn Sum<T>(this IColumn column)
        {
            column.Add(CreateSumCalculator(typeof (T)));
            return column;
        }

        /// <summary>
        /// avg all rows belong this column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public static IColumn Avg<T>(this IColumn column)
        {
            column.Add(CreateAvgCalculator(typeof (T)));
            return column;
        }

        private static ICalculator CreateAvgCalculator(Type t)
        {
            return AvgMap[t].Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal static ICalculator CreateSumCalculator(Type t)
        {
            return SumMap[t].Invoke();
        }

        /// <summary>
        /// Get sum result from column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static object SumResult(this IColumn column)
        {
            if (column.HasCaculator(CalculatorBase<int>.SumCalculatorName))
            {
                return column.GetResult(CalculatorBase<int>.SumCalculatorName);
            }
            throw new ArgumentException(String.Format("Column {0} do not set the Sum function.",
                                                      CalculatorBase<int>.SumCalculatorName));
        }

        /// <summary>
        /// Get avg result from column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static object AvgResult(this IColumn column)
        {
            if (column.HasCaculator(CalculatorBase<int>.AvgCalculatorName))
            {
                return column.GetResult(CalculatorBase<int>.AvgCalculatorName);
            }
            throw new ArgumentException(String.Format("Column {0} do not set the Sum function.",
                                                      CalculatorBase<int>.AvgCalculatorName));
        }
    }
}
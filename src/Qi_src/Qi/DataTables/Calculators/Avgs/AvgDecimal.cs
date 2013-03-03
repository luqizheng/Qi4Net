using System;

namespace Qi.DataTables.Calculators.Avgs
{
    internal class AvgDecimal : Avg<decimal>
    {
        public AvgDecimal()
            : base(Convert.ToDecimal, (a, b) => a + b)
        {
        }


        internal static AvgDecimal Create()
        {
            return new AvgDecimal();
        }
    }
}
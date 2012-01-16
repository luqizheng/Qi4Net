using System;

namespace Qi.DataTables.Calculators.Avgs
{
    internal class AvgDecimalNullable : Avg<decimal?>
    {
        public AvgDecimalNullable()
            : base(s => Convert.ToDecimal(s), (a, b) => (a ?? 0) + (b ?? 0))
        {
        }

        internal static AvgDecimalNullable Create()
        {
            return new AvgDecimalNullable();
        }

    
    }
}
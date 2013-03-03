using System;

namespace Qi.DataTables.Calculators.Avgs
{
    internal class AvgDoubleNullable : Avg<double?>
    {
        public AvgDoubleNullable()
            : base(s => Convert.ToDouble(s), (a, b) => (a ?? 0) + (b ?? 0))
        {
        }

        internal static AvgDoubleNullable Create()
        {
            return new AvgDoubleNullable();
        }

       
    }
}
using System;

namespace Qi.DataTables.Calculators.Sums
{
    internal class SumDoubleNullable : Sum<double?>
    {
        public SumDoubleNullable()
            : base(s => Convert.ToDouble(s), (a, b) => (a ?? 0) + (b ?? 0))
        {
        }

        internal static SumDoubleNullable Create()
        {
            return new SumDoubleNullable();
        }
    }
}
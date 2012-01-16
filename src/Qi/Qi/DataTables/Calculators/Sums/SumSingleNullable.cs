using System;

namespace Qi.DataTables.Calculators.Sums
{
    internal class SumSingleNullable : Sum<float?>
    {
        public SumSingleNullable()
            : base(s => Convert.ToInt64(s), (a, b) => (a ?? 0) + (b ?? 0))
        {
        }

        internal static SumSingleNullable Create()
        {
            return new SumSingleNullable();
        }
    }
}
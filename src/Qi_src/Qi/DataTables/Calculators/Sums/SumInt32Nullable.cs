using System;

namespace Qi.DataTables.Calculators.Sums
{
    internal class SumInt32Nullable : Sum<int?>
    {
        public SumInt32Nullable()
            : base(s => Convert.ToInt32(s), (a, b) => (a ?? 0) + (b ?? 0))
        {
        }

        internal static SumInt32Nullable Create()
        {
            return new SumInt32Nullable();
        }
    }
}
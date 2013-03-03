using System;

namespace Qi.DataTables.Calculators.Sums
{
    internal class SumInt64Nullable : Sum<long?>
    {
        public SumInt64Nullable()
            : base(s => Convert.ToInt64(s), (a, b) => (a ?? 0) + (b ?? 0))
        {
        }

        internal static SumInt64Nullable Create()
        {
            return new SumInt64Nullable();
        }
    }
}
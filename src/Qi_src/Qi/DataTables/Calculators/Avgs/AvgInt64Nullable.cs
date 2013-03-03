using System;

namespace Qi.DataTables.Calculators.Avgs
{
    internal class AvgInt64Nullable : Avg<long?>
    {
        public AvgInt64Nullable()
            : base(s => Convert.ToInt64(s), (a, b) => (a ?? 0 + (b ?? 0)))
        {

        }
        internal static AvgInt64Nullable Create()
        {
            return new AvgInt64Nullable();
        }

    }
}
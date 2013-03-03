using System;

namespace Qi.DataTables.Calculators.Avgs
{
    internal class AvgInt32Nullable : Avg<int?>
    {
        public AvgInt32Nullable()
            : base(s => Convert.ToInt32(s ?? 0), (a, b) => (a ?? 0) + (b ?? 0))
        {

        }
        internal static AvgInt32Nullable Create()
        {
            return new AvgInt32Nullable();
        }

    }
}
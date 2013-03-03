using System;

namespace Qi.DataTables.Calculators.Avgs
{
    internal class AvgInt64 : Avg<long>
    {
        public AvgInt64()
            : base(Convert.ToInt64, (a, b) => a + b)
        {

        }
        internal static AvgInt64 Create()
        {
            return new AvgInt64();
        }

    }
}
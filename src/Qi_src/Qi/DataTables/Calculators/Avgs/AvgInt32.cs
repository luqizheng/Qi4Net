using System;

namespace Qi.DataTables.Calculators.Avgs
{
    internal class AvgInt32 : Avg<int>
    {
        public AvgInt32()
            : base(Convert.ToInt32, (a, b) => a + b)
        {

        }
        internal static AvgInt32 Create()
        {
            return new AvgInt32();
        }

    }
}
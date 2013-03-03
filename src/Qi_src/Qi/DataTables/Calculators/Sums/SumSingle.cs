using System;

namespace Qi.DataTables.Calculators.Sums
{
    internal class SumSingle : Sum<float>
    {
        public SumSingle()
            : base(Convert.ToSingle, (a, b) => a + b)
        {
        }

        internal static SumSingle Create()
        {
            return new SumSingle();
        }
    }
}
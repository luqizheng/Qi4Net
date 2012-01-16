using System;

namespace Qi.DataTables.Calculators.Sums
{
    internal class SumDouble : Sum<double>
    {
        public SumDouble()
            : base(Convert.ToDouble, (a, b) => a + b)
        {
        }

        internal static SumDouble Create()
        {
            return new SumDouble();
        }
    }
}
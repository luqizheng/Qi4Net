using System;

namespace Qi.DataTables.Calculators.Sums
{
    internal class SumInt32 : Sum<int>
    {
        public SumInt32()
            : base(Convert.ToInt32, (a, b) => a + b)
        {
        }

        internal static ICalculator Create()
        {
            return new SumInt32();
        }
    }
   
}
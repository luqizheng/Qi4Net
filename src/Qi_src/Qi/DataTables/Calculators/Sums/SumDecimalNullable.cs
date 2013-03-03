using System;

namespace Qi.DataTables.Calculators.Sums
{
    internal class SumDecimalNullable : Sum<decimal?>
    {
        public SumDecimalNullable():base(s=>Convert.ToDecimal(s),(a,b)=>(a ?? 0) + (b ?? 0))
        {
        }

        internal static SumDecimalNullable Create()
        {
            return new SumDecimalNullable();
        }
    }
}
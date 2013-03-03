using System;

namespace Qi.DataTables.Calculators
{
    internal abstract class Sum<T> : CalculatorBase<T>
    {
        
        protected Sum(Func<object, T> convertor, Func<T, T, T> calculate)
            : base(convertor, calculate)
        {
        }

        public override string Name
        {
            get { return SumCalculatorName; }
        }
    }
}
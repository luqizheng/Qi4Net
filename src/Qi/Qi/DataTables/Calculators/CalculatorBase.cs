using System;

namespace Qi.DataTables.Calculators
{
    internal abstract class CalculatorBase<T> : ICalculator
    {

        internal const string SumCalculatorName = "Sum";
        internal const string AvgCalculatorName = "Avg";

        private readonly Func<T, T, T> _calculate;
        private readonly Func<object, T> _convertor;

        protected CalculatorBase(Func<object, T> convertor, Func<T, T, T> calculate)
        {
            if (convertor == null) throw new ArgumentNullException("convertor");
            if (calculate == null) throw new ArgumentNullException("calculate");
            _convertor = convertor;
            _calculate = calculate;
        }

        #region ICalculator Members

        public virtual object Result { get; private set; }

        public abstract string Name { get; }

        public virtual void SetValue(object rowValue)
        {
            Result = _calculate(Result == null ? default(T) : (T)Result, _convertor(rowValue));
        }

        public void Clear()
        {
            Result = default(T);
        }

        #endregion
    }
}
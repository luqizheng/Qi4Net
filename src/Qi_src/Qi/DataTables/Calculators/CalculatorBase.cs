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

        private object _result;
        public virtual object Result
        {
            get { return _result; }
            private set { _result = value; }
        }

        public abstract string Name { get; }

        public virtual void SetValue(object rowValue)
        {
            _result = _calculate(_result == null ? default(T) : (T)_result, _convertor(rowValue));
        }

        public virtual void Clear()
        {
            _result = default(T);
        }

        #endregion
    }
}
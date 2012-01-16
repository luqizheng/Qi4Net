using System;

namespace Qi.DataTables.Calculators
{
    internal abstract class Avg<T> : Sum<T>
    {
        private int _count;

        protected Avg(Func<object, T> convertor, Func<T, T, T> calculate)
            : base(convertor, calculate)
        {
        }

        public override object Result
        {
            get
            {
                if (_count != 0)
                {
                    return getAvag(Convert.ToDecimal(base.Result), _count);
                }
                return default(T);
            }
        }

        public override string Name
        {
            get { return CalculatorBase<T>.AvgCalculatorName; }
        }

        public override void SetValue(object rowValue)
        {
            base.SetValue(rowValue);
            _count++;
        }

        private object getAvag(decimal total, int count)
        {
            return total / count;
        }
    }
}
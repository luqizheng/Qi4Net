using System;

namespace Qi.DataTables.Columns
{
    internal class CustomeCalculatorColumn<TReturnValue> : AbstractColumn<TReturnValue>
    {
        private readonly Func<IColumn[], TReturnValue> _calculator;
        private readonly IColumn[] _columns;

        public CustomeCalculatorColumn(string name, Func<IColumn[], TReturnValue> calculator, params IColumn[] columns)
            : base(name)
        {
            _calculator = calculator;
            _columns = columns;
        }

        protected override object InvokeObject(object rowObject)
        {
            return _calculator(_columns);
        }
    }
}
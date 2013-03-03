using System;

namespace Qi.DataTables.Columns
{
    internal class CustomeCalculatorColumn<TReturnValue> : AbstractColumn<TReturnValue>
    {
        private readonly Func<object[], TReturnValue> _calculator;
        private readonly IColumn[] _columns;

        public CustomeCalculatorColumn(string name, Func<object[], TReturnValue> calculator, params IColumn[] columns)
            : base(name)
        {
            _calculator = calculator;
            _columns = columns;
        }

        protected override object InvokeObject(object rowObject)
        {
            var columnValue = new object[_columns.Length];
            int i = 0;
            foreach (var a in _columns)
            {
                columnValue[i] = a.GetValue(rowObject);
                i++;
            }
            return _calculator(columnValue);
        }
    }
}
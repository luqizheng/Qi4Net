namespace Qi.DataTables.Columns
{
    internal class CalculatorColumn<TReturnValue> : AbstractColumn<TReturnValue>
    {
        private readonly ICalculator _calculator;
        private readonly IColumn[] _columns;

        public CalculatorColumn(string name, ICalculator calculator, IColumn[] columns)
            : base(name)
        {
            _calculator = calculator;
            _columns = columns;
        }

        protected override object InvokeObject(object rowObject)
        {
            foreach (IColumn col in _columns)
            {
                _calculator.SetValue(col.GetValue(rowObject));
            }
            var result = (TReturnValue) _calculator.Result;
            _calculator.Clear();
            return result;
        }
    }
}
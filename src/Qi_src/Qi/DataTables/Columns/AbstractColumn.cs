using System;
using System.Collections.Generic;

namespace Qi.DataTables.Columns
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">T for colum's Value type.</typeparam>
    public abstract class AbstractColumn<T> : IColumn
    {
        private object _cacheData;
        private int _rowObjectHasCode;
        private SortedDictionary<string, ICalculator> _sets;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected AbstractColumn(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            Name = name;
        }

        private IDictionary<string, ICalculator> Calculators
        {
            get { return _sets ?? (_sets = new SortedDictionary<string, ICalculator>()); }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return Calculators.Count; }
        }

        #region IColumn Members
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object IColumn.GetValue(object data)
        {
            return GetValue(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorName"></param>
        /// <returns></returns>
        public bool HasCaculator(string calculatorName)
        {
            return Calculators.ContainsKey(calculatorName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorName"></param>
        /// <returns></returns>
        public object GetResult(string calculatorName)
        {
            if (Calculators.ContainsKey(calculatorName))
            {
                return Calculators[calculatorName].Result;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorIndex"></param>
        /// <returns></returns>
        public object GetResult(int calculatorIndex)
        {
            if (_sets == null)
                return null;
            int i = 0;
            foreach (ICalculator item in _sets.Values)
            {
                if (i == calculatorIndex)
                    return item.Result;
                i++;
            }
            return null;
        }


        /// <summary>
        /// Reset all the result include calculator
        /// </summary>
        public void Reset()
        {
            _cacheData = null;
            foreach (ICalculator a in Calculators.Values)
            {
                a.Clear();
            }
        }

        /// <summary>
        /// clear the reference of type,because it may be object.
        /// </summary>
        public void Clear()
        {
            _cacheData = null;
        }

        /// <summary>
        /// Add new calulator for this Column
        /// </summary>
        /// <param name="result"></param>
        public void Add(ICalculator result)
        {
            if (result == null)
                throw new ArgumentNullException("result");
            Calculators.Add(result.Name, result);
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public T GetValue(object data)
        {
            bool sameRowObject = _rowObjectHasCode == data.GetHashCode();

            _cacheData = sameRowObject ? _cacheData : InvokeObject(data);

            if (_sets != null && !sameRowObject)
            {
                _rowObjectHasCode = data.GetHashCode();
                foreach (ICalculator calculator in Calculators.Values)
                {
                    calculator.SetValue(_cacheData);
                }
            }
            return (T) _cacheData;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        protected abstract object InvokeObject(object rowObject);
    }
}
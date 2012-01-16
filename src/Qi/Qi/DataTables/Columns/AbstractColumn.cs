using System;
using System.Collections.Generic;
using Qi.DataTables.Calculators;

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

        protected AbstractColumn(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            Name = name;
        }

        private IDictionary<string, ICalculator> Sets
        {
            get { return _sets ?? (_sets = new SortedDictionary<string, ICalculator>()); }
        }

        #region IColumn Members

        public string Name { get; set; }

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
            return Sets.ContainsKey(calculatorName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorName"></param>
        /// <returns></returns>
        public object GetResult(string calculatorName)
        {
            if (Sets.ContainsKey(calculatorName))
            {
                return Sets[calculatorName].Result;
            }
            return null;
        }

        public object GetResult(int calculatorIndex)
        {
            if (_sets == null)
                return null;
            int i = 0;
            foreach (var item in _sets.Values)
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
            foreach (ICalculator a in Sets.Values)
            {
                a.Clear();
            }
        }

        /// <summary>
        /// clear the reference of the <see cref="T"/>,because it may be object.
        /// </summary>
        public void Clear()
        {
            _cacheData = null;
        }

        public void Add(ICalculator result)
        {
            if (result == null)
                throw new ArgumentNullException("result");
            Sets.Add(result.Name, result);
        }

        #endregion

        public T GetValue(object data)
        {
            bool sameRowObject = _rowObjectHasCode == data.GetHashCode();

            _cacheData = sameRowObject ? _cacheData : InvokeObject(data);

            if (_sets != null && !sameRowObject)
            {
                _rowObjectHasCode = data.GetHashCode();
                foreach (ICalculator item in Sets.Values)
                {
                    item.SetValue(_cacheData);
                }
            }
            return (T)_cacheData;
        }

        protected abstract object InvokeObject(object rowObject);
    }
}
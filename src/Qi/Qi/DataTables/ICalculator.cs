namespace Qi.DataTables
{
    public interface ICalculator
    {
        /// <summary>
        /// Gets or sets the name of calculator
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Get the result which calculated.
        /// </summary>
        object Result { get; }

        /// <summary>
        /// set value to this object for calculate;
        /// </summary>
        /// <param name="rowValue"></param>
        void SetValue(object rowValue);

        /// <summary>
        /// 
        /// </summary>
        void Clear();
    }
}
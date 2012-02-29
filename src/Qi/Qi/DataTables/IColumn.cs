namespace Qi.DataTables
{
    public interface IColumn
    {
        /// <summary>
        /// Gets or sets the name of this column
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the Calulator's count
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object GetValue(object data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorName"></param>
        /// <returns></returns>
        bool HasCaculator(string calculatorName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorName"></param>
        /// <returns></returns>
        object GetResult(string calculatorName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculatorIndex"></param>
        /// <returns></returns>
        object GetResult(int calculatorIndex);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculator"></param>
        void Add(ICalculator calculator);

        /// <summary>
        /// Clear the result of the summary and the value
        /// </summary>
        void Clear();

        /// <summary>
        /// Reset result of the calulators
        /// </summary>
        void Reset();
    }
}
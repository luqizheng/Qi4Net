namespace Qi.SharePools
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetData(string key, object data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetData(string key);
    }
    internal interface IStoreFactory
    {
        IStore Store { get; }
    }
}

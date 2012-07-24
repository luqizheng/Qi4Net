namespace Qi.SharePools
{
    public interface IStore
    {
        void SetData(string key, object data);
        object GetData(string key);
    }
}

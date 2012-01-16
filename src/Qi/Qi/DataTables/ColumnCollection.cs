using System.Collections.ObjectModel;

namespace Qi.DataTables
{
    public class ColumnCollection : KeyedCollection<string, IColumn>
    {
        protected override string GetKeyForItem(IColumn item)
        {
            return item.Name;
        }
    }
}
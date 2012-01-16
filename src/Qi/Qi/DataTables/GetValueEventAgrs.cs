using System;

namespace Qi.DataTables
{
    public class GetValueEventAgrs : EventArgs
    {
        public GetValueEventAgrs(object value, object rowData)
        {
            Value = value;
            RowData = rowData;
        }

        public object Value { get; private set; }
        public object RowData { get; private set; }
    }
}
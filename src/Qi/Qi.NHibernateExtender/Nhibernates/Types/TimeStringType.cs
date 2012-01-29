using System;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.SqlTypes;

namespace Qi.Nhibernates.Types
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeStringType : AbstractTimeType
    {
        public TimeStringType()
            : base(SqlTypeFactory.GetAnsiString(8))
        {
        }
        public override Type PrimitiveClass
        {
            get { return typeof(string); }
        }
        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            var val = (Time)value;
            return val.ToString();
        }

        public override object Get(IDataReader rs, string name)
        {
            for (int i = 0; i < rs.FieldCount; i++)
            {
                if (rs.GetName(i) == name)
                    return Get(rs, i);
            }
            return new Time();
        }

        public override object Get(IDataReader rs, int index)
        {
            var result = rs[index] != null ? ToTime(rs.GetString(index)) : ToTime("00:00:00");
            return (object)result;
        }

        public override void Set(IDbCommand cmd, object value, int index)
        {
            var type = (Time)value;
            var param = (DbParameter)cmd.Parameters[index];
            param.Value = type.ToString();
        }

    }
}
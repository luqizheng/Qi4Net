using System;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.SqlTypes;

namespace Qi.Nhibernates.Types
{
    /// <summary>
    /// Use Int64 to store Time 
    /// </summary>
    public class TimeType : AbstractTimeType
    {
        public TimeType()
            : base(SqlTypeFactory.Int64)
        {
        }

        public override Type PrimitiveClass
        {
            get { return typeof(Int64); }
        }

        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            var val = (Time)value;
            return val.Ticks.ToString();
        }
        public override object Get(IDataReader rs, string name)
        {
            return new Time(Convert.ToInt64(rs[name].ToString()));
        }

        public override object Get(IDataReader rs, int index)
        {
            return new Time(rs.GetInt64(index));
        }

        public override void Set(IDbCommand cmd, object value, int index)
        {
            var type = (Time)value;
            DbParameter param = cmd.Parameters[index] as DbParameter;
            param.Value = type.Ticks;
        }


    }
}
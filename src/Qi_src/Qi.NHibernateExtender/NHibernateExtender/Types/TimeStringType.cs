using System;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.SqlTypes;

namespace Qi.NHibernateExtender.Types
{
    /// <summary>
    ///     Use string like "00:00:00" to store time.
    /// </summary>
    public class TimeStringType : AbstractTimeType
    {
        /// <summary>
        /// </summary>
        public TimeStringType()
            : base(SqlTypeFactory.GetAnsiString(8))
        {
        }

        /// <summary>
        /// </summary>
        public override Type PrimitiveClass
        {
            get { return typeof (string); }
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dialect"></param>
        /// <returns></returns>
        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            var val = (Time) value;
            return val.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object Get(IDataReader rs, string name)
        {
            for (int i = 0; i < rs.FieldCount; i++)
            {
                if (rs.GetName(i) == name)
                    return Get(rs, i);
            }
            return new Time();
        }

        /// <summary>
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override object Get(IDataReader rs, int index)
        {
            Time result = rs[index] != null ? ToTime(rs.GetString(index)) : ToTime("00:00:00");
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public override void Set(IDbCommand cmd, object value, int index)
        {
            var type = (Time) value;
            var param = (DbParameter) cmd.Parameters[index];
            param.Value = type.ToString();
        }
    }
}
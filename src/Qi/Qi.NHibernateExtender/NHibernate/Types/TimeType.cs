using System;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.SqlTypes;

namespace Qi.NHibernate.Types
{
    /// <summary>
    /// Use Int64 to store Time 
    /// </summary>
    public class TimeType : AbstractTimeType
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeType()
            : base(SqlTypeFactory.Int64)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override Type PrimitiveClass
        {
            get { return typeof(Int64); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dialect"></param>
        /// <returns></returns>
        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            var val = (Time)value;
            return val.Ticks.ToString();
        }

        /// <summary>
        /// When implemented by a class, gets the <see cref="Time"/> object in the 
        ///             <see cref="T:System.Data.IDataReader"/> for the Property.
        /// </summary>
        /// <param name="rs">The <see cref="T:System.Data.IDataReader"/> that contains the value.</param><param name="name">The name of the field to get the value from.</param>
        /// <returns>
        /// An object with the value from the database.
        /// </returns>
        /// <remarks>
        /// Most implementors just call the <see cref="M:NHibernate.Type.NullableType.Get(System.Data.IDataReader,System.Int32)"/> 
        ///             overload of this method.
        /// </remarks>
        public override object Get(IDataReader rs, string name)
        {
            return new Time(Convert.ToInt64(rs[name].ToString()));
        }

        /// <summary>
        /// When implemented by a class, gets the <see cref="Time"/> object in the 
        ///             <see cref="T:System.Data.IDataReader"/> for the Property.
        /// </summary>
        /// <param name="rs">The <see cref="T:System.Data.IDataReader"/> that contains the value.</param><param name="index">The index of the field to get the value from.</param>
        /// <returns>
        /// An object with the value from the database.
        /// </returns>
        public override object Get(IDataReader rs, int index)
        {
            return new Time(rs.GetInt64(index));
        }

        /// <summary>
        /// When implemented by a class, put the value from the mapped 
        ///             Property into to the <see cref="T:System.Data.IDbCommand"/>.
        /// </summary>
        /// <param name="cmd">The <see cref="T:System.Data.IDbCommand"/> to put the value into.</param><param name="value">The object that contains the value.</param><param name="index">The index of the <see cref="T:System.Data.IDbDataParameter"/> to start writing the values to.</param>
        /// <remarks>
        /// Implementors do not need to handle possibility of null values because this will
        ///             only be called from <see cref="M:NHibernate.Type.NullableType.NullSafeSet(System.Data.IDbCommand,System.Object,System.Int32)"/> after 
        ///             it has checked for nulls.
        /// </remarks>
        public override void Set(IDbCommand cmd, object value, int index)
        {
            var type = (Time)value;
            DbParameter param = cmd.Parameters[index] as DbParameter;
            param.Value = type.Ticks;
        }


    }
}
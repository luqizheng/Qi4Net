using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Qi.Web;

namespace Qi.Nhibernates.Types
{
    public class KeyValueCollectionType : NHibernate.Type.PrimitiveType
    {
        public KeyValueCollectionType()
            : base(new NHibernate.SqlTypes.StringClobSqlType())
        {

        }
        public override object DefaultValue
        {
            get { return new Dictionary<string, object>(); }
        }

        public override string ObjectToSQLString(object value, NHibernate.Dialect.Dialect dialect)
        {
            throw new NotImplementedException("KeyValueCollectionType Not Implemented ObjectToSQLString");
        }

        public override Type PrimitiveClass
        {
            get { return typeof(Dictionary<string, string>); }
        }

        public override object FromStringValue(string xml)
        {
            return JsonContainer.Create(xml).Content;
        }

        public override object Get(System.Data.IDataReader rs, string name)
        {
            for (int i = 0; i < rs.FieldCount; i++)
            {
                if (rs.GetName(i) == name)
                {
                    return Get(rs, i);
                }
            }
            return new Dictionary<string, object>();
        }

        public override object Get(System.Data.IDataReader rs, int index)
        {
            return FromStringValue(rs[index].ToString());
        }

        public override void Set(System.Data.IDbCommand cmd, object value, int index)
        {
            var param = (System.Data.IDbDataParameter)cmd.Parameters[index];
            var val = (Dictionary<string, object>)value;
            param.Value = val.ToJson();
        }

        public override string Name
        {
            get { return "KeyValueCollection"; }
        }

        public override Type ReturnedClass
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }
}

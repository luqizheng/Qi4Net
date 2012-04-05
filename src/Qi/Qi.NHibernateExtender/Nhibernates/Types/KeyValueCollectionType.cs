using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NHibernate.Type;
using Qi.Web;

namespace Qi.Nhibernates.Types
{
    public class KeyValueCollectionType : PrimitiveType
    {
        public KeyValueCollectionType()
            : base(new StringClobSqlType())
        {
        }

        public override object DefaultValue
        {
            get { return new Dictionary<string, object>(); }
        }

        public override Type PrimitiveClass
        {
            get { return typeof (Dictionary<string, string>); }
        }

        public override string Name
        {
            get { return "KeyValueCollection"; }
        }

        public override Type ReturnedClass
        {
            get { return typeof (Dictionary<string, object>); }
        }

        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            var content = (IDictionary<string, object>) value;
            return content.ToString();
        }

        public override object FromStringValue(string xml)
        {
            return JsonContainer.Create(xml).Content;
        }

        public override object Get(IDataReader rs, string name)
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

        public override object Get(IDataReader rs, int index)
        {
            return FromStringValue(rs[index].ToString());
        }

        public override void Set(IDbCommand cmd, object value, int index)
        {
            var param = (IDbDataParameter) cmd.Parameters[index];
            var val = (Dictionary<string, object>) value;
            param.Value = val.ToJson();
        }

        public override int Compare(object x, object y, EntityMode? entityMode)
        {
            var a = (IDictionary<string, object>) x;
            var b = (IDictionary<string, object>) y;
            if (a.Count == b.Count)
                return 0;
            string aJson = a.ToJson(false);
            string bJson = b.ToJson(false);
            return String.CompareOrdinal(aJson, bJson);
        }
    }
}
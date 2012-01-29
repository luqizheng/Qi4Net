using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

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
            get { return new Dictionary<string, string>(); }
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
            var result = new Dictionary<string, string>();
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(xml);
            foreach (XmlNode node in dom.SelectNodes("i"))
            {
                result.Add(node.Attributes["k"].Value, node.InnerText);
            }
            return result;
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
            return new Dictionary<string, string>();
        }

        public override object Get(System.Data.IDataReader rs, int index)
        {
            var butes = (byte[])rs[index];
            return FromStringValue(BitConverter.ToString(butes));
        }

        public override void Set(System.Data.IDbCommand cmd, object value, int index)
        {
            var param = (System.Data.IDbDataParameter)cmd.Parameters[index];
            var val = (Dictionary<string, string>)value;
            StringBuilder sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?><keyValue>");

            foreach (var item in val)
            {
                sb.Append("<i k=\"").Append(item.Key)
                    .Append("\">").Append(item.Value)
                    .Append("</i>");
            }
            sb.Append("</keyValue>");

            param.Value = sb.ToString();
        }

        public override string Name
        {
            get { return "KeyValueCollection"; }
        }

        public override Type ReturnedClass
        {
            get
            {
                return typeof(Dictionary<string, string>);
            }
        }
    }
}

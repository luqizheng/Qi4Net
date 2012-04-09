using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Engine;
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
            get { return typeof(Dictionary<string, string>); }
        }

        public override string Name
        {
            get { return "KeyValueCollection"; }
        }

        public override Type ReturnedClass
        {
            get { return typeof(Dictionary<string, object>); }
        }

        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            var content = (IDictionary<string, object>)value;
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
            var param = (IDbDataParameter)cmd.Parameters[index];
            var dict = value as IDictionary<string, object>;
            param.Value = dict == null ? value : dict.ToJson(false);
        }

        public override object DeepCopy(object value, EntityMode entityMode, ISessionFactoryImplementor factory)
        {
            var dict = value as IDictionary<string, object>;
            if (dict != null)
                return dict.ToJson(false);
            return value.ToString();
        }

        public override bool IsEqual(object x, object y, EntityMode entityMode)
        {
            var yDict = y as Dictionary<string, object>;
            if (yDict == null)
            {
                yDict = JsonContainer.Create(y.ToString()).Content;
            }

            var xDict = x as Dictionary<string, object>;
            if (xDict == null)
            {
                xDict = JsonContainer.Create(x.ToString()).Content;
            }
            if (xDict.Count != yDict.Count)
                return false;
            foreach (var xKey in xDict.Keys)
            {
                if (!yDict.ContainsKey(xKey) || yDict[xKey].Equals(xDict[xKey]))
                    return false;
            }
            return true;
        }

        public override int Compare(object x, object y, EntityMode? entityMode)
        {
            var dict = y as Dictionary<string, object>;
            if (dict != null)
            {
                return dict.ToJson(false).CompareTo(x.ToString());
            }
            else
            {
                dict = x as Dictionary<string, object>;
                return dict.ToJson(false).CompareTo(y.ToString());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using Qi.Web;

namespace Qi.NHibernate.Types
{
    /// <summary>
    /// Dictionary for nh mapping, Dictionary that key is string,value is object
    /// object is a json string database.
    /// </summary>
    public class KeyValueCollectionType : PrimitiveType
    {
        /// <summary>
        /// 
        /// </summary>
        public KeyValueCollectionType()
            : base(new StringClobSqlType())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override object DefaultValue
        {
            get { return new Dictionary<string, object>(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Type PrimitiveClass
        {
            get { return typeof (Dictionary<string, string>); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get { return "KeyValueCollection"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Type ReturnedClass
        {
            get { return typeof (Dictionary<string, object>); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dialect"></param>
        /// <returns></returns>
        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            var content = (IDictionary<string, object>) value;
            return content.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public override object FromStringValue(string xml)
        {
            return JsonContainer.Create(xml).Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object Get(IDataReader rs, string name)
        {
            return rs[name];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override object Get(IDataReader rs, int index)
        {
            return FromStringValue(rs[index].ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public override void Set(IDbCommand cmd, object value, int index)
        {
            var param = (IDbDataParameter) cmd.Parameters[index];
            var dict = value as IDictionary<string, object>;
            param.Value = dict == null ? value : dict.ToJson(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="entityMode"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public override object DeepCopy(object value, EntityMode entityMode, ISessionFactoryImplementor factory)
        {
            var dict = value as IDictionary<string, object>;
            if (dict != null)
                return dict.ToJson(false);
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="entityMode"></param>
        /// <returns></returns>
        public override bool IsEqual(object x, object y, EntityMode entityMode)
        {
            Dictionary<string, object> yDict = y as Dictionary<string, object> ??
                                               JsonContainer.Create(y.ToString()).Content;

            Dictionary<string, object> xDict = x as Dictionary<string, object> ??
                                               JsonContainer.Create(x.ToString()).Content;

            if (xDict.Count != yDict.Count)
                return false;
            return xDict.Keys.All(xKey => yDict.ContainsKey(xKey) && !yDict[xKey].Equals(xDict[xKey]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="entityMode"></param>
        /// <returns></returns>
        public override int Compare(object x, object y, EntityMode? entityMode)
        {
            Dictionary<string, object> yDict = y as Dictionary<string, object> ??
                                               JsonContainer.Create(y.ToString()).Content;

            Dictionary<string, object> xDict = x as Dictionary<string, object> ??
                                               JsonContainer.Create(x.ToString()).Content;
            if (xDict.Count != yDict.Count)
                return xDict.Count.CompareTo(yDict.Count);
            if (xDict.Keys.Any(xKey => !yDict.ContainsKey(xKey) || yDict[xKey].Equals(xDict[xKey])))
            {
                return -1;
            }
            return 0;
        }
    }
}
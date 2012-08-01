using System;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Qi.Nhibernates.Types
{
    /// <summary>
    /// A abstract time type for nhibernate mapping
    /// </summary>
    public abstract class AbstractTimeType : PrimitiveType
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlType"></param>
        protected AbstractTimeType(SqlType sqlType) : base(sqlType)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public override sealed object DefaultValue
        {
            get { return new Time(); }
        }
        /// <summary>
        /// 
        /// </summary>
        public override sealed string Name
        {
            get { return "Time"; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override sealed Type ReturnedClass
        {
            get { return typeof (Time); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public override object FromStringValue(string xml)
        {
            return ToTime(xml);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected static Time ToTime(string s)
        {
            string[] aryS = s.Split(':');
            return new Time(Convert.ToInt32(aryS[0]), Convert.ToInt32(aryS[1]), Convert.ToInt32(aryS[2]));
        }
    }
}
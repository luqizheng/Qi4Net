using System;
using NHibernate.Type;
using NHibernate.SqlTypes;

namespace Qi.Nhibernates.Types
{
    public abstract class AbstractTimeType : PrimitiveType
    {
        protected AbstractTimeType(SqlType sqlType):base(sqlType)
        {
     
        }

        public sealed override object DefaultValue
        {
            get { return new Time(); }
        }

        public sealed override string Name
        {
            get { return "Time"; }
        }

        public  sealed override Type ReturnedClass
        {
            get { return typeof(Time); }
        }

        public override object FromStringValue(string xml)
        {
            return ToTime(xml);
        }

        protected static Time ToTime(string s)
        {
            string[] aryS = s.Split(':');
            return new Time(Convert.ToInt32(aryS[0]), Convert.ToInt32(aryS[1]), Convert.ToInt32(aryS[2]));
        }

    }
}

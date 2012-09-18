using System;
using NHibernate;
using NHibernate.Criterion;

namespace Qi.NHibernate.Criterion
{
    /// <summary>
    /// 
    /// </summary>
    public class BitwiseFlags : LogicalExpression
    {
        private BitwiseFlags(string propertyName, object value, string op) :
            base(new SimpleExpression(propertyName, value, op),
                 Expression.Sql("?", value, NHibernateUtil.Enum(value.GetType())))
        {
        }

        private BitwiseFlags(ICriterion lhs, ICriterion rhs)
            : base(lhs, rhs)
        {

        }
        protected override string Op
        {
            get { return "="; }
        }

        public static BitwiseFlags IsSet(string propertyName, Enum flags)
        {
            return new BitwiseFlags(propertyName, flags, " & ");
        }

        public static BitwiseFlags IsSet(string propertyName, int flags)
        {
            var lhs = new SimpleExpression(propertyName, flags, " & ");
            var rhs = Expression.Sql("?", flags, NHibernateUtil.Int32);
            return new BitwiseFlags(lhs, rhs);
        }
    }
}
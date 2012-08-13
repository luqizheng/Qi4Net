using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using NHibernate;
using NHibernate.Type;
using Qi.NHibernate;

namespace Qi.Web.Mvc.Founders
{
    public class HqlFounderAttribute : FounderAttribute
    {
        private readonly string _hql;
        private readonly string _postDataType;

        public HqlFounderAttribute(string hql, string postDataType)
        {
            Unique = false;

            _hql = hql;
            _postDataType = postDataType;
        }

        public HqlFounderAttribute(string hql, string postDataType, string[] anotherParameterType,
                                   string[] anotherParameterName)
            : this(hql, postDataType)
        {
            AnotherParameterName = anotherParameterName;
            AnotherParameterType = anotherParameterType;
        }

        public new bool Unique
        {
            get { return base.Unique; }
            set { base.Unique = value; }
        }

        public string[] AnotherParameterType { get; set; }
        public string[] AnotherParameterName { get; set; }


        protected override IList GetObject(ISession session, object[] id, string postName, NameValueCollection context)
        {
            var result = new List<object>();
            IQuery crit = session.CreateQuery(_hql);
            if (Unique)
            {
                crit.SetMaxResults(1).SetFirstResult(1);
            }
            IType hqlType = TypeFactory.Basic(_postDataType);
            if (AnotherParameterName != null)
            {
                CreateParameter(context, crit);
            }
            foreach (object condition in id)
            {
                crit.SetParameter(postName, condition, hqlType);
                result.AddRange(crit.List<object>());
            }

            return result;
        }

        private void CreateParameter(NameValueCollection context, IQuery crit)
        {
            int i = 0;
            foreach (string parameterName in AnotherParameterName)
            {
                string strValu = context[parameterName];
                IType paramType = TypeFactory.Basic(AnotherParameterType[i]);
                if (paramType == null)
                    throw new NhConfigurationException("Nhibernate do not defined base type named " +
                                                       AnotherParameterType[i]);

                object val = NHMappingHelper.ConvertStringToObject(strValu, paramType);
                crit.SetParameter(parameterName, val, paramType);
            }
        }

        public override IType GetMappingType(ISession session, string requestKey)
        {
            return TypeFactory.Basic(_postDataType);
        }
    }
}
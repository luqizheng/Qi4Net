using System.Web;
using NHibernate;
using NHibernate.Type;
using Qi.Nhibernates;

namespace Qi.Web.Mvc.Founders
{
    public class HqlFounderAttribute : FounderAttribute
    {
        private readonly string _hql;
        private readonly string _postDataType;

        public HqlFounderAttribute(string hql, string postDataType)
        {
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

        public string[] AnotherParameterType { get; set; }
        public string[] AnotherParameterName { get; set; }


        protected override object GetObject(SessionManager sessionManager, object postData, string postName,
                                            HttpContextBase context)
        {
            IQuery crit = sessionManager.CurrentSession.CreateQuery(_hql);

            IType hqlType = TypeFactory.Basic(_postDataType);
            crit.SetParameter(postName, postData, hqlType);
            if (AnotherParameterName != null)
            {
                CreateParameter(context, crit);
            }
            if (!Unique)
                crit.SetFirstResult(0).SetMaxResults(1);
            return crit.UniqueResult();
        }

        private void CreateParameter(HttpContextBase context, IQuery crit)
        {
            int i = 0;
            foreach (string parameterName in AnotherParameterName)
            {
                string strValu = context.Request[parameterName];
                IType paramType = TypeFactory.Basic(AnotherParameterType[i]);
                if (paramType == null)
                    throw new NhConfigurationException("Nhibernate do not defined base type named " +
                                                       AnotherParameterType[i]);

                object val = ConvertStringToObject(strValu, paramType);
                crit.SetParameter(parameterName, val, paramType);
            }
        }

        protected override IType PostDataType(SessionManager sessionManager, string postDataName)
        {
            return TypeFactory.Basic(_postDataType);
        }
    }
}
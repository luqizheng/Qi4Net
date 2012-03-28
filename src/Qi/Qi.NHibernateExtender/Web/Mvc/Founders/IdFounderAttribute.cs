using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Type;
using Qi.Nhibernates;

namespace Qi.Web.Mvc.Founders
{
    public class IdFounderAttribute : FounderAttribute
    {
        protected override object GetObject(ISession session, object postData, string postName,
                                            HttpContextBase context)
        {
            return DetachedCriteria.For(EntityType).Add(Restrictions.Eq(Projections.Id(), postData)).GetExecutableCriteria(session).UniqueResult();

        }

        protected override IType PostDataType(ISession sessionManager, string postDataName)
        {
            Configuration nh = NhConfigManager.GetNhConfig(SessionManager.CurrentSessionFactoryKey).NHConfiguration;
            return nh.GetClassMapping(EntityType).Identifier.Type;
        }
    }
}
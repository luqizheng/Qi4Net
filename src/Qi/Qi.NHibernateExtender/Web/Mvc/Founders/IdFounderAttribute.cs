using System.Collections;
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
        public IdFounderAttribute()
        {
            Unique = false;
        }

        protected override IList GetObject(ISession session, object[] id, string postName, HttpContextBase context)
        {
            var disJun = new Disjunction();
            foreach (var ids in id)
            {
                disJun.Add(Restrictions.Eq(Projections.Id(), ids));
            }
            return (ArrayList)DetachedCriteria.For(EntityType).Add(disJun).GetExecutableCriteria(session).List();
        }

        public override IType GetMappingType(ISession sessionManager, string requestKey)
        {
            var type =
                SessionManager.Instance.GetSessionFactory(this.EntityType).GetClassMetadata(this.EntityType).
                    IdentifierType;
            return type;
        }
    }
}
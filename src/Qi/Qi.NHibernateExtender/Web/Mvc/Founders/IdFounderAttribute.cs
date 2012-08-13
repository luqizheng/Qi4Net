using System.Collections;
using System.Collections.Specialized;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Type;
using Qi.NHibernate;

namespace Qi.Web.Mvc.Founders
{
    public class IdFounderAttribute : FounderAttribute
    {
        public IdFounderAttribute()
        {
            Unique = false;
        }

        protected override IList GetObject(ISession session, object[] id, string postName, NameValueCollection context)
        {
            var disJun = new Disjunction();
            foreach (var ids in id)
            {
                disJun.Add(Restrictions.Eq(Projections.Id(), ids));
            }
            return DetachedCriteria.For(EntityType).Add(disJun).GetExecutableCriteria(session).List();
        }

        public override IType GetMappingType(ISession session, string requestKey)
        {
            return session.SessionFactory.GetClassMetadata(this.EntityType).IdentifierType;
        }
    }
}
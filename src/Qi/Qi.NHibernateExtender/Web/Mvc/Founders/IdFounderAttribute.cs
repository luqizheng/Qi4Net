using System.Collections;
using System.Collections.Specialized;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Type;
using Qi.Nhibernates;

namespace Qi.Web.Mvc.Founders
{
    /// <summary>
    /// Use Id of Mapping class to find the object whihc belong a property or field defined in a DTO
    /// </summary>
    public class IdFounderAttribute : FounderAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public IdFounderAttribute()
        {
            Unique = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="id"></param>
        /// <param name="postName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override IList GetObject(ISession session, object[] id, string postName, NameValueCollection context)
        {
            var disJun = new Disjunction();
            foreach (var ids in id)
            {
                disJun.Add(Restrictions.Eq(Projections.Id(), ids));
            }
            return DetachedCriteria.For(EntityType).Add(disJun).GetExecutableCriteria(session).List();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestKey"></param>
        /// <returns></returns>
        public override IType GetMappingType(ISession session, string requestKey)
        {
            return session.SessionFactory.GetClassMetadata(this.EntityType).IdentifierType;
        }
    }
}
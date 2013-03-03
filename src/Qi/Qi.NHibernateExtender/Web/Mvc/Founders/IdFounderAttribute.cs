using System.Collections;
using System.Web.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Type;

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
        protected override IList GetObject(ISession session, object[] id, string postName, ModelBindingContext context)
        {
            var disJun = new Disjunction();
            foreach (object ids in id)
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
            return session.SessionFactory.GetClassMetadata(EntityType).IdentifierType;
        }
    }
}
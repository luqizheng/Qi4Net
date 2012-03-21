using System.Web;
using NHibernate.Type;
using Qi.Nhibernates;

namespace Qi.Web.Mvc.Founders
{
    public class IdFounderAttribute : FounderAttribute
    {
        protected override object GetObject(SessionManager sessionManager, object postData, string postName,
                                            HttpContextBase context)
        {
            var a = sessionManager.CurrentSession.Load(EntityType, postData);
            sessionManager.CurrentSession.Evict(a);
            return a;
        }

        protected override IType PostDataType(SessionManager sessionManager, string postDataName)
        {
            return sessionManager.Config.NHConfiguration.GetClassMapping(EntityType).Identifier.Type;
        }
    }
}
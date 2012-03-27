using System.Web;
using NHibernate;
using NHibernate.Type;
using Qi.Nhibernates;

namespace Qi.Web.Mvc.Founders
{
    public class IdFounderAttribute : FounderAttribute
    {
        protected override object GetObject(ISession sessionManager, object postData, string postName,
                                            HttpContextBase context)
        {
            var a = sessionManager.Load(EntityType, postData);
            sessionManager.Evict(a);
            return a;
        }

        protected override IType PostDataType(ISession sessionManager, string postDataName)
        {
            var nh=NhConfigManager.GetNhConfig(SessionManager.CurrentSessionFactoryKey).NHConfiguration;
            return nh.GetClassMapping(EntityType).Identifier.Type;
        }
    }
}
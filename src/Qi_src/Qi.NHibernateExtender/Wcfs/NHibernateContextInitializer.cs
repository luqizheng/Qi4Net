using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Qi.Wcfs
{
    /// <summary>
    /// 
    /// </summary>
    public class NHibernateContextInitializer : IInstanceContextInitializer
    {
        private readonly bool _enabledTransaction;
        private readonly string _sessionfactoryName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionfactoryName"></param>
        /// <param name="enabledTransaction"></param>
        public NHibernateContextInitializer(string sessionfactoryName, bool enabledTransaction)
        {
            _sessionfactoryName = sessionfactoryName;
            _enabledTransaction = enabledTransaction;
        }

        public void Initialize(InstanceContext instanceContext, Message message)
        {
            instanceContext.Extensions.Add(new NHibernateContextExension(_sessionfactoryName, _enabledTransaction));
        }
    }
}
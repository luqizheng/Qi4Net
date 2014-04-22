using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Qi.NHibernateExtender;

namespace Qi.Wcfs
{
    /// <summary>
    ///     WCF service context for hibernate.
    /// </summary>
    public class NHibernateContextAttribute : Attribute, IContractBehavior
    {
        private readonly bool _enabledSession;
        private readonly string _sessionFactoryName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabledSession"></param>
        /// <param name="sessionFactoryName"></param>
        public NHibernateContextAttribute(bool enabledSession, string sessionFactoryName)
        {
            _enabledSession = enabledSession;
            _sessionFactoryName = sessionFactoryName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabledSession"></param>
        public NHibernateContextAttribute(bool enabledSession)
            : this(enabledSession, SessionManager.DefaultSessionFactoryKey)
        {
        }

        /// <summary>
        ///     Default to disabled transaction and use defualt sessionfactory key
        /// </summary>
        public NHibernateContextAttribute()
            : this(false)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractDescription"></param>
        /// <param name="endpoint"></param>
        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="contractDescription"></param>
        /// <param name="endpoint"></param>
        /// <param name="dispatchRuntime"></param>
        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceContextInitializers.Add(new NHibernateContextInitializer(_sessionFactoryName,
                _enabledSession));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractDescription"></param>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractDescription"></param>
        /// <param name="endpoint"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }
    }
}
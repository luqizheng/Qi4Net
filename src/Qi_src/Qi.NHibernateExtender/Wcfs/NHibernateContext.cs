using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Qi.NHibernateExtender;

namespace Qi.Wcfs
{
    /// <summary>
    /// WCF service context for hibernate.
    /// </summary>
    public class NHibernateContextAttribute : Attribute, IContractBehavior
    {
        private readonly bool _enabledSession;
        private readonly string _sessionFactoryName;

        public NHibernateContextAttribute(bool enabledSession, string sessionFactoryName)
        {
            _enabledSession = enabledSession;
            _sessionFactoryName = sessionFactoryName;
        }

        public NHibernateContextAttribute(bool enabledSession)
            : this(enabledSession, SessionManager.DefaultSessionFactoryKey)
        {

        }
        /// <summary>
        /// Default to disabled transaction and use defualt sessionfactory key
        /// </summary>
        public NHibernateContextAttribute()
            : this(false)
        {

        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractDescription"></param>
        /// <param name="endpoint"></param>
        /// <param name="dispatchRuntime"></param>
        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
                                          DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceContextInitializers.Add(new NHibernateContextInitializer(_sessionFactoryName, _enabledSession));
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
                                         BindingParameterCollection bindingParameters)
        {
        }
    }
}

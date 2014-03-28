using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using NHibernate;
using Qi.NHibernateExtender;

namespace Qi.Wcfs
{
    /// <summary>
    /// 
    /// </summary>
    public class NHibernateContextExension : IExtension<InstanceContext>, IErrorHandler
    {
        private readonly bool _enabledTransaction;
        private readonly string _sessionfactoryName;
        private SessionWrapper _sessionWrapper;
        private ITransaction _trans;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionfactoryName"></param>
        /// <param name="enabledTransaction"></param>
        public NHibernateContextExension(string sessionfactoryName, bool enabledTransaction)
        {
            _sessionfactoryName = sessionfactoryName;
            _enabledTransaction = enabledTransaction;
        }


        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {

        }

        public bool HandleError(Exception error)
        {
            _trans.Rollback();
            return true;
        }

        public void Attach(InstanceContext owner)
        {
            _sessionWrapper = SessionManager.GetSessionWrapper(_sessionfactoryName);
            if (_enabledTransaction)
            {
                _trans = _sessionWrapper.CurrentSession.BeginTransaction();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public void Detach(InstanceContext owner)
        {
            if (_enabledTransaction)
            {
                _trans.Commit();
            }
            _sessionWrapper.CurrentSession.Flush();
        }
    }
}
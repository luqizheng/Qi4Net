using System;
using NHibernate;
using NHibernate.Cfg;

namespace Qi.NHibernate
{
    public abstract class NhConfig : INhConfig
    {
        private Configuration _nhConfiguration;
        private string _sessionFactoryName;

        protected NhConfig()
        {
        }

        protected NhConfig(string sessionFactoryName, Configuration cfg)
        {
            if (sessionFactoryName == null) 
                throw new ArgumentNullException("sessionFactoryName");
            if (cfg == null)
                throw new ArgumentNullException("cfg");
            _sessionFactoryName = sessionFactoryName;
            _nhConfiguration = cfg;
        }

        #region INhConfig Members

        /// <summary>
        /// Gets the SessionFactoryName 
        /// </summary>
        public string SessionFactoryName
        {
            get
            {
                if (IsChanged || String.IsNullOrEmpty(_sessionFactoryName))
                {
                    lock (this)
                    {
                        if (IsChanged || String.IsNullOrEmpty(_sessionFactoryName))
                        {
                            Refresh();
                        }
                    }
                }
                return _sessionFactoryName;
            }
        }

        /// <summary>
        /// Gets the Nhibernate Configuration from this setting file.
        /// </summary>
        public Configuration NHConfiguration
        {
            get
            {
                if (IsChanged || _nhConfiguration == null)
                {
                    lock (this)
                    {
                        if (IsChanged || _nhConfiguration == null)
                        {
                            Refresh();
                        }
                    }
                }
                return _nhConfiguration;
            }
        }

        /// <summary>
        /// Get the value indecate file is changed or not.
        /// </summary>
        public abstract bool IsChanged { get; }

        /// <summary>
        /// 
        /// </summary>
        public ISessionFactory BuildSessionFactory()
        {
            try
            {
                return NHConfiguration.BuildSessionFactory();
            }
            catch (Exception ex)
            {
                string msg =string.Format("session factory config [name={0}] throw exception:{1}", SessionFactoryName, ex.Message);
                throw new NhConfigurationException(msg ,ex);
            }
        }

        /// <summary>
        /// Refresh config target,it may be a file. and re-build the session name and configuration.
        /// </summary>
        public virtual void Refresh()
        {
            lock (this)
            {
                _sessionFactoryName = GetSessionFactoryName();
                _nhConfiguration = BuildConfiguration();
                ResetToUnChanged();
            }
        }

        #endregion

        /// <summary>
        /// if apply changed, it will call this function to reset changed state to un-changed.
        /// </summary>
        protected abstract void ResetToUnChanged();

        /// <summary>
        /// get the sessionFacotry name
        /// </summary>
        /// <returns></returns>
        protected abstract string GetSessionFactoryName();

        /// <summary>
        /// Build Configuration 
        /// </summary>
        /// <returns></returns>
        protected abstract Configuration BuildConfiguration();
    }
}
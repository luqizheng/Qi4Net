using NHibernate;
using NHibernate.Cfg;

namespace Qi.NHibernate
{
    public interface INhConfig
    {
        /// <summary>
        /// Gets the SessionFactoryName 
        /// </summary>
        string SessionFactoryName { get; }

        /// <summary>
        /// Gets the Nhibernate Configuration from this setting file.
        /// </summary>
        Configuration NHConfiguration { get; }

        /// <summary>
        /// Get the value indecate file is changed or not.
        /// </summary>
        bool IsChanged { get; }

        /// <summary>
        /// 
        /// </summary>
        ISessionFactory BuildSessionFactory();

        /// <summary>
        /// 
        /// </summary>
        void Refresh();
    }
}
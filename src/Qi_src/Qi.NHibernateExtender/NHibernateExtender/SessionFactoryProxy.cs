using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;

namespace Qi.NHibernateExtender
{
    /// <summary>
    /// </summary>
    public class SessionFactoryProxy
    {
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="name"></param>
        public SessionFactoryProxy(Configuration configuration, string name)
        {
            Configuration = configuration;
            _sessionFactory = configuration.BuildSessionFactory();
            Name = name;
        }

        /// <summary>
        /// </summary>
        public Configuration Configuration { get; private set; }

        /// <summary>
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        /// <summary>
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Open it and close itself.
        /// </summary>
        /// <returns></returns>
        public SessionWrapper CreateNewWrapper()
        {
            SessionProxy session = null;
            if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                session = new SessionProxy(SessionFactory.OpenSession());
            }
            else
            {
                var parent = (SessionProxy) CurrentSessionContext.Unbind(SessionFactory);
                session = new SessionProxy(SessionFactory.OpenSession(), parent);
            }

            CurrentSessionContext.Bind(session);
            return new SessionWrapper(SessionFactory);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public SessionWrapper GetWrapper()
        {
            if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                return CreateNewWrapper();
            }
            return new SessionWrapper(SessionFactory);
        }
    }
}
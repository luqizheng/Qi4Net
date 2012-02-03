using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Qi.Nhibernates;

namespace Qi.Domain.NHibernates
{
    /// <summary>
    /// Dao base for nhibernate implement
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    public abstract class DaoBase<TId, TObject> : IDao<TObject, TId>
    {
        private readonly bool _useGlobalSession;
        private ISession _privateSession;

        /// <summary>
        /// 
        /// </summary>
        protected DaoBase()
            : this(true)
        {
        }
        
        protected DaoBase(string sessionFactoryName, bool useGlobalSession)
        {
            SessionFactoryName = sessionFactoryName;
            _useGlobalSession = useGlobalSession;
        }

        protected DaoBase(string sessionFactoryName)
        {
            SessionFactoryName = sessionFactoryName;
        }

        protected DaoBase(bool useGlobalSession)
        {
            _useGlobalSession = useGlobalSession;
        }

        public string SessionFactoryName { get; private set; }

        protected ISession CurrentSession
        {
            get
            {
                if (_useGlobalSession)
                {
                    return GetSessionManager().CurrentSession;
                }
                return _privateSession ??
                       (_privateSession = GetSessionManager().NewSession);
            }
        }

        protected IStatelessSession StatelessSession
        {
            get { return GetSessionManager().Sessionless; }
        }

        #region IDao<TObject,TId> Members

        public virtual TObject Get(TId id)
        {
            return CurrentSession.Get<TObject>(id);
        }

        public virtual TObject Load(TId id)
        {
            return CurrentSession.Load<TObject>(id);
        }

        public virtual IList<TObject> GetAll()
        {
            return CreateDetachedCriteria().GetExecutableCriteria(CurrentSession).List<TObject>();
        }

        public void Update(TObject t)
        {
            CurrentSession.Update(t);
        }


        public virtual void Delete(TObject t)
        {
            CurrentSession.Delete(t);
        }

        public virtual void Refresh(TObject t)
        {
            CurrentSession.Refresh(t);
        }

        public virtual void SaveOrUpdate(TObject t)
        {
            CurrentSession.SaveOrUpdate(t);
        }

        public virtual TId Save(TObject t)
        {
            return (TId) CurrentSession.Save(t);
        }

        public virtual IList<TObject> FindByExample(TObject exampleInstance, params string[] propertiesToExclude)
        {
            ICriteria criteria = CreateCriteria();
            Example example = Example.Create(exampleInstance);

            foreach (string propertyToExclude in propertiesToExclude)
            {
                example.ExcludeProperty(propertyToExclude);
            }

            criteria.Add(example);

            return criteria.List<TObject>();
        }

        public void Flush()
        {
            CurrentSession.Flush();
        }

        public void Dispose()
        {
            if (!_useGlobalSession && _privateSession != null)
            {
                _privateSession.Close();
            }
        }

        #endregion

        private SessionManager GetSessionManager()
        {
            SessionManager result;
            if (!string.IsNullOrEmpty(SessionFactoryName))
            {
                result = SessionManager.GetInstance(SessionFactoryName);
            }
            else if (SessionManager.Instance.Config.NHConfiguration.GetClassMapping(typeof (TObject)) != null)
            {
                result = SessionManager.Instance;
            }
            else
            {
                result = SessionManager.GetInstance(typeof (TObject));
            }
            SessionFactoryName = result.Config.SessionFactoryName; // set it and make quick in next time.
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected DetachedCriteria CreateDetachedCriteria()
        {
            return DetachedCriteria.For(typeof (TObject));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual ICriteria CreateCriteria()
        {
            return CurrentSession.CreateCriteria(typeof (TObject));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual IQuery CreateQuery(string query)
        {
            return CurrentSession.CreateQuery(query);
        }
    }
}
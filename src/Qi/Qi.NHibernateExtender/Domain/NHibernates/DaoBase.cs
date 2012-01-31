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
        private readonly string _sessionFactoryName;
        private readonly bool _useGlobalSession;
        private ISession _privateSession;


        protected DaoBase()
            : this(true)
        {
        }

        protected DaoBase(string sessionFactoryName, bool useGlobalSession)
        {
            _sessionFactoryName = sessionFactoryName;
            _useGlobalSession = useGlobalSession;
        }

        protected DaoBase(string sessionFactoryName)
        {
            _sessionFactoryName = sessionFactoryName;
        }

        protected DaoBase(bool useGlobalSession)
        {
            _useGlobalSession = useGlobalSession;
        }


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
            return !string.IsNullOrEmpty(_sessionFactoryName)
                       ? SessionManager.GetInstance(_sessionFactoryName)
                       : SessionManager.GetInstance(typeof (TObject));
        }

        protected DetachedCriteria CreateDetachedCriteria()
        {
            return DetachedCriteria.For(typeof (TObject));
        }


        protected virtual ICriteria CreateCriteria()
        {
            return CurrentSession.CreateCriteria(typeof (TObject));
        }

        protected virtual IQuery CreateQuery(string query)
        {
            return CurrentSession.CreateQuery(query);
        }
    }
}
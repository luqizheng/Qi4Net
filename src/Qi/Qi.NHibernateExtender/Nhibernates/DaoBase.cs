using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Qi.Domain;

namespace Qi.Nhibernates
{
    public abstract class DaoBase<TId, TObject> : IDao<TObject, TId>
    {
        private readonly bool _useGlobalSession;
        private ISession _privateSession;

        protected DaoBase()
            : this(true)
        {
        }


        protected DaoBase(bool useGlobalSession)
        {
            _useGlobalSession = useGlobalSession;
        }

        protected virtual Order[] DefaultOrder
        {
            get { return new[] {Order.Asc(Projections.Id())}; }
        }

        protected ISession CurrentSession
        {
            get
            {
                if (_useGlobalSession)
                    return SessionManager.Instance.CurrentSession;
                return _privateSession ?? (_privateSession = SessionManager.Instance.NewSession);
            }
        }

        protected IStatelessSession StatelessSession
        {
            get { return SessionManager.Instance.Sessionless; }
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

        public virtual IList<TObject> FindAll(int startRow, int pageSize)
        {
            if (startRow < 0)
                throw new ArgumentOutOfRangeException("startRow", startRow, "Less than zero");
            if (pageSize < 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "Less than zero");
            DetachedCriteria cri = DetachedCriteria.For(typeof (TObject))
                .SetMaxResults(pageSize).SetFirstResult(startRow);
            return
                cri.GetExecutableCriteria(CurrentSession).List<TObject>();
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

        public virtual int CountAll()
        {
            return CreateCriteria().SetProjection(Projections.Count(Projections.Id())).UniqueResult<int>();
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

        protected DetachedCriteria CreateDetachedCriteria()
        {
            return DetachedCriteria.For(typeof (TObject));
        }

        protected virtual IList<TObject> Find(int pageIndex, int pageSize, ICriteria ica)
        {
            return ica.SetMaxResults(pageSize).SetFirstResult(pageIndex*pageSize).List<TObject>();
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
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
    public abstract class DaoBase<TId, TObject> : IDao<TId, TObject> where TObject : DomainObject<TObject, TId>
    {
        private readonly SessionWrapper _wrapper;

        protected DaoBase(string sessionFactoryName)
        {
            _wrapper = SessionManager.GetSessionWrapper(sessionFactoryName);
        }

        protected DaoBase()
            : this(SessionManager.Instance.CurrentSessionFactoryName ?? SessionManager.DefaultSessionFactoryKey)
        {
        }

        protected ISession CurrentSession
        {
            get { return _wrapper.CurrentSession; }
        }

        #region IDao<TId,TObject> Members

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

        #endregion

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
using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Qi.NHibernateExtender;

namespace Qi.Domain.NHibernates
{
    /// <summary>
    ///     Dao base for nhibernate implement
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    public abstract class DaoBase<TId, TObject> : IDao<TId, TObject> where TObject : DomainObject<TObject, TId>
    {
        private readonly SessionWrapper _wrapper;

        /// <summary>
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <exception cref="ArgumentNullException">sessionFactoryName is null or empty</exception>

        //protected DaoBase(string sessionFactoryName)
        //{
        //    if (string.IsNullOrEmpty(sessionFactoryName))
        //    {
        //        throw new ArgumentNullException("sessionFactoryName", "sessionFactoryName can not be null or empty");
        //    }
        //    _wrapper = SessionManager.GetSessionWrapper(sessionFactoryName);
        //}

        /// <summary>
        /// </summary>
        protected DaoBase()
        {
            _wrapper = SessionManager.GetSessionWrapper();
        }

        /// <summary>
        /// </summary>
        protected ISession CurrentSession
        {
            get { return _wrapper.CurrentSession; }
        }

        #region IDao<TId,TObject> Members

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TObject Get(TId id)
        {
            return CurrentSession.Get<TObject>(id);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TObject Load(TId id)
        {
            return CurrentSession.Load<TObject>(id);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public virtual IList<TObject> GetAll()
        {
            return CreateDetachedCriteria().GetExecutableCriteria(CurrentSession).List<TObject>();
        }

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        public void Update(TObject t)
        {
            CurrentSession.Update(t);
        }

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        public virtual void Delete(TObject t)
        {
            CurrentSession.Delete(t);
        }

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        public virtual void Refresh(TObject t)
        {
            CurrentSession.Refresh(t);
        }

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        public virtual void SaveOrUpdate(TObject t)
        {
            CurrentSession.SaveOrUpdate(t);
        }

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual TId Save(TObject t)
        {
            return (TId)CurrentSession.Save(t);
        }

        /// <summary>
        /// </summary>
        /// <param name="exampleInstance"></param>
        /// <param name="propertiesToExclude"></param>
        /// <returns></returns>
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


        /// <summary>
        /// </summary>
        public void Flush()
        {
            CurrentSession.Flush();
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return
                CreateDetachedCriteria()
                    .SetProjection(Projections.RowCount()).SetCacheMode(CacheMode.Get)
                    .GetExecutableCriteria(CurrentSession)
                    .UniqueResult<int>();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected DetachedCriteria CreateDetachedCriteria()
        {
            return DetachedCriteria.For(typeof(TObject));
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected virtual ICriteria CreateCriteria()
        {
            return CurrentSession.CreateCriteria(typeof(TObject));
        }

        /// <summary>
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual IQuery CreateQuery(string query)
        {
            return CurrentSession.CreateQuery(query);
        }
    }
}
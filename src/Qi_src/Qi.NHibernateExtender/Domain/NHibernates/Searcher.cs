using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using Qi.NHibernateExtender;

namespace Qi.Domain.NHibernates
{
    /// <summary>
    ///     NHibernate Search
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class Searcher<TObject, TId> where TObject : DomainObject<TObject, TId>
    {
        private readonly SessionWrapper _wrapper;

        /// <summary>
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        protected Searcher(string sessionFactoryName)
        {
            if (string.IsNullOrEmpty(sessionFactoryName))
            {
                throw new ArgumentNullException("sessionFactoryName");
            }
            _wrapper = SessionManager.GetSessionWrapper(sessionFactoryName);
            Criteria = DetachedCriteria.For<TObject>();
            CountCriteria = DetachedCriteria.For<TObject>();
        }

        /// <summary>
        /// </summary>
        protected Searcher()
            : this(SessionManager.Instance.CurrentSessionFactoryName ?? SessionManager.DefaultSessionFactoryKey)
        {
        }

        protected DetachedCriteria Criteria { get; set; }

        protected DetachedCriteria CountCriteria { get; set; }


        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<TObject> Find(int start, int length, out int total)
        {
            CountCriteria.SetProjection(Projections.RowCount());
            Criteria.SetMaxResults(length).SetFirstResult(start);

            Criteria.GetExecutableCriteria(_wrapper.CurrentSession)
                .UniqueResult<int>();


            total = CountCriteria.GetExecutableCriteria(_wrapper.CurrentSession).UniqueResult<int>();
            return Criteria.GetExecutableCriteria(_wrapper.CurrentSession)
                .List<TObject>();
        }
    }
}
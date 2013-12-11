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
        }

        /// <summary>
        /// </summary>
        protected Searcher()
            : this(SessionManager.Instance.CurrentSessionFactoryName ?? SessionManager.DefaultSessionFactoryKey)
        {
        }

        /// <summary>
        ///     Sort Property
        /// </summary>
        public virtual SortTarget[] Sort { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<TObject> Find(int start, int length, out int total)
        {
            DetachedCriteria result = CreateCriteria();

            total =
                result.SetProjection(Projections.RowCount())
                    .GetExecutableCriteria(_wrapper.CurrentSession)
                    .UniqueResult<int>();

            result = CreateCriteria();
            IList<TObject> t =
                result.SetMaxResults(length)
                    .SetFirstResult(start)
                    .GetExecutableCriteria(_wrapper.CurrentSession)
                    .List<TObject>();
            return t;
        }

        protected virtual DetachedCriteria CreateCriteria()
        {
            DetachedCriteria result = DetachedCriteria.For(typeof (TObject));
            BuildSortProperty(result);
            return result;
        }

        protected virtual void BuildSortProperty(DetachedCriteria result)
        {
            if (Sort != null && Sort.Length != 0)
            {
                foreach (SortTarget a in Sort)
                {
                    result.AddOrder(a.Tag == SortDirect.Asc
                        ? Order.Asc(Projections.Property(a.Property))
                        : Order.Desc(Projections.Property(a.Property)));
                }
            }
        }
    }
}
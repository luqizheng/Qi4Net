using System;
using System.Collections.Generic;
using System.Data;

namespace Qi.Domain
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TId">
    ///     Id of the TReturn;
    /// </typeparam>
    /// <typeparam name="TReturn">Domain object's typel</typeparam>
    public interface IDao<TId, TReturn> : IDisposable where TReturn : DomainObject<TReturn, TId>
    {
        /// <summary>
        ///     Get All object
        /// </summary>
        /// <returns></returns>
        IList<TReturn> GetAll();

        /// <summary>
        ///     Return the persistent instance of the given entity class with the given identifier, or null if there is no such
        ///     persistent instance. (If the instance, or a proxy for the instance, is already associated with the session, return
        ///     that instance or proxy.)
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <returns>
        ///     a persistent instance or null
        /// </returns>
        TReturn Get(TId id);

        /// <summary>
        ///     Return the persistent instance of the given entity class with the given identifier, assuming that the instance
        ///     exists.
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <returns>
        ///     The persistent instance or proxy
        /// </returns>
        /// <remarks>
        ///     You should not use this method to determine if an instance exists (use a query or
        ///     NHibernate.ISession.Get(System.Type,System.Object) instead). Use this only to retrieve an instance that you assume
        ///     exists, where non-existence would be an actual error.
        /// </remarks>
        TReturn Load(TId id);

        /// <summary>
        ///     Delete the record of an entity from Database and thus the entity becomes transient
        /// </summary>
        /// <param name="t">
        /// </param>
        void Delete(TReturn t);

        /// <summary>
        ///     Re-read the state of the entity from the database
        /// </summary>
        /// <param name="t">
        /// </param>
        void Refresh(TReturn t);

        /// <summary>
        ///     Persist the entity <paramref name="t" /> to DB if it has not been persisted before
        /// </summary>
        /// <param name="t">
        /// </param>
        /// <remarks>
        ///     By default the instance is always saved.
        ///     This behaviour may be adjusted by specifying an unsaved-value attribute of the identifier property mapping
        /// </remarks>
        void SaveOrUpdate(TReturn t);

        /// <summary>
        ///     Persist the given transient instance, first assigning a generated identifier.
        /// </summary>
        /// <param name="t">
        ///     the given transient instance
        /// </param>
        /// <returns>
        ///     The generated identifier
        /// </returns>
        /// <remarks>
        ///     Save will use the current value of the identifier property if the Assigned generator is used.
        /// </remarks>
        TId Save(TReturn t);

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        void Update(TReturn t);


        /// <summary>
        /// </summary>
        /// <param name="exampleInstance">
        ///     The example instance.
        /// </param>
        /// <param name="propertiesToExclude">
        ///     The properties to exclude.
        /// </param>
        /// <returns>
        /// </returns>
        IList<TReturn> FindByExample(TReturn exampleInstance, params string[] propertiesToExclude);

        /// <summary>
        /// </summary>
        void Flush();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// 
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        void BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// 
        /// </summary>
        void Commit();

        /// <summary>
        /// 
        /// </summary>
        void RollBack();

        /// <summary>
        /// πÿ±’¡¥Ω”
        /// </summary>
        /// <returns></returns>
        bool Close();
    }
}
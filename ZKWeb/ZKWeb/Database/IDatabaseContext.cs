using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace ZKWeb.Database {
	/// <summary>
	/// Interface for database context
	/// </summary>
	public interface IDatabaseContext : IDisposable {
		/// <summary>
		/// Begin a transaction
		/// It should support recursive calling
		/// Only the first call will create a transaction, later calls just increase the counter
		/// Attention: Not all ORM support this feature
		/// </summary>
		/// <param name="isolationLevel">The isolation level</param>
		void BeginTransaction(IsolationLevel? isolationLevel = null);

		/// <summary>
		/// Finish the transaction
		/// It should support recursive calling
		/// Only the last call will commit the transaction, previous calls just decrease the counter
		/// Attention: Not all ORM support this feature
		/// </summary>
		void FinishTransaction();

		/// <summary>
		/// Get the query object for specific entity
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <returns></returns>
		IQueryable<T> Query<T>()
			where T : class, IEntity;

		/// <summary>
		/// Get single entity that matched the given predicate
		/// It should return null if no matched entity found
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <returns></returns>
		T Get<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity;

		/// <summary>
		/// Get how many entities that matched the given predicate
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <returns></returns>
		long Count<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity;

		/// <summary>
		/// Save entity to database
		/// It should call registered `IEntityOperationHandler`
		/// Update action can be used for operation between `BeforeSave` and `AfterSave` callback
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="entity">Entity object, may get replaced if not tracked by ORM</param>
		/// <param name="update">Update action</param>
		void Save<T>(ref T entity, Action<T> update = null)
			where T : class, IEntity;

		/// <summary>
		/// Delete entity from database
		/// It should call registered `IEntityOperationHandler`
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="entity">Entity object</param>
		void Delete<T>(T entity)
			where T : class, IEntity;

		/// <summary>
		/// Batch save entities
		/// It should call registered `IEntityOperationHandler`
		/// Update action can be used for operation between `BeforeSave` and `AfterSave` callback
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="entities">Entity object</param>
		/// <param name="update">Update action</param>
		void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
			where T : class, IEntity;

		/// <summary>
		/// Batch update entities
		/// Return how many entities get updated
		/// It should call registered `IEntityOperationHandler`
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <param name="update">Update action</param>
		/// <returns></returns>
		long BatchUpdate<T>(Expression<Func<T, bool>> predicate, Action<T> update)
			where T : class, IEntity;

		/// <summary>
		/// Batch delete entities
		/// Return how many entities get deleted
		/// It should call registered `IEntityOperationHandler`
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <returns></returns>
		long BatchDelete<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity;

		/// <summary>
		/// Perform a raw update to database
		/// No operation handler will be called
		/// `query` and `parameters` are ORM and Database specified
		/// </summary>
		/// <param name="query">Query object (eg: string)</param>
		/// <param name="parameters">Query parameters (eg: IDictionary[String, object])</param>
		/// <returns></returns>
		long RawUpdate(object query, object parameters);

		/// <summary>
		/// Perform a raw query to database
		/// No operation handler will be called
		/// `query` and `parameters` are ORM and Database specified
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="query">Query object (eg: string)</param>
		/// <param name="parameters">Query parameters (eg: SqlParameter[])</param>
		/// <returns></returns>
		IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class;
	}
}

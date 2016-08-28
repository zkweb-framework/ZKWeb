using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using ZKWeb.Database;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// NHibernate database context
	/// </summary>
	internal class NHibernateDatabaseContext : IDatabaseContext {
		/// <summary>
		/// NHibernate session
		/// </summary>
		private ISession Session { get; set; }
		/// <summary>
		/// NHibernate transaction, maybe null if not used
		/// </summary>
		private ITransaction Transaction { get; set; }
		/// <summary>
		/// Transaction level counter
		/// </summary>
		private int TransactionLevel;

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="session">NHibernate session</param>
		public NHibernateDatabaseContext(ISession session) {
			Session = session;
			Transaction = null;
			TransactionLevel = 0;
		}

		/// <summary>
		/// Finalize
		/// </summary>
		~NHibernateDatabaseContext() {
			Dispose();
		}

		/// <summary>
		/// Dispose nhibernate session and transaction
		/// </summary>
		public void Dispose() {
			Transaction?.Dispose();
			Transaction = null;
			Session?.Dispose();
			Session = null;
		}

		/// <summary>
		/// Begin a transaction
		/// </summary>
		public void BeginTransaction(IsolationLevel? isolationLevel) {
			var level = Interlocked.Increment(ref TransactionLevel);
			if (level == 1) {
				if (Transaction != null) {
					throw new InvalidOperationException("Transaction exists");
				}
				Transaction = (isolationLevel == null) ?
					Session.BeginTransaction() :
					Session.BeginTransaction(isolationLevel.Value);
			}
		}

		/// <summary>
		/// Finish the transaction
		/// </summary>
		public void FinishTransaction() {
			var level = Interlocked.Decrement(ref TransactionLevel);
			if (level == 0) {
				if (Transaction == null) {
					throw new InvalidOperationException("Transaction not exists");
				}
				Transaction.Commit();
				Transaction.Dispose();
				Transaction = null;
			} else if (level < 0) {
				Interlocked.Exchange(ref level, 0);
				throw new InvalidOperationException(
					"You can't call FinishTransaction more times than BeginTransaction");
			}
		}

		/// <summary>
		/// Get the query object for specific entity
		/// </summary>
		public IQueryable<T> Query<T>()
			where T : class, IEntity {
			return Session.Query<T>();
		}

		/// <summary>
		/// Get single entity that matched the given predicate
		/// </summary>
		public T Get<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			return Query<T>().FirstOrDefault(predicate);
		}

		/// <summary>
		/// Get how many entities that matched the given predicate
		/// </summary>
		public long Count<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			return Query<T>().LongCount(predicate);
		}

		/// <summary>
		/// Save entity to database
		/// </summary>
		public void Save<T>(ref T entity, Action<T> update = null)
			where T : class, IEntity {
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			var entityLocal = entity; // can't use ref parameter in lambda
			callbacks.ForEach(c => c.BeforeSave(this, entityLocal)); // notify before save
			update?.Invoke(entityLocal);
			entityLocal = Session.Merge(entityLocal);
			Session.Flush(); // send commands to database
			callbacks.ForEach(c => c.AfterSave(this, entityLocal)); // notify after save
			entity = entityLocal;
		}

		/// <summary>
		/// Delete entity from database
		/// </summary>
		public void Delete<T>(T entity)
			where T : class, IEntity {
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			callbacks.ForEach(c => c.BeforeDelete(this, entity)); // notify before delete
			Session.Delete(entity);
			Session.Flush(); // send commands to database
			callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
		}

		/// <summary>
		/// Batch save entities
		/// </summary>
		public void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
			where T : class, IEntity {
			var entitiesLocal = entities.ToList();
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			for (int i = 0; i < entitiesLocal.Count; ++i) {
				var entity = entitiesLocal[i];
				callbacks.ForEach(c => c.BeforeSave(this, entity)); // notify before save
				update?.Invoke(entity);
				entity = Session.Merge(entity);
				entitiesLocal[i] = entity;
			}
			Session.Flush(); // send commands to database
			foreach (var entity in entitiesLocal) {
				callbacks.ForEach(c => c.AfterSave(this, entity)); // notify after save
			}
			entities = entitiesLocal;
		}

		/// <summary>
		/// Batch update entities
		/// </summary>
		public long BatchUpdate<T>(Expression<Func<T, bool>> predicate, Action<T> update)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).AsEnumerable();
			BatchSave(ref entities, update);
			return entities.LongCount();
		}

		/// <summary>
		/// Batch delete entities
		/// </summary>
		public long BatchDelete<T>(Expression<Func<T, bool>> predicate, Action<T> beforeDelete)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).ToList();
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			foreach (var entity in entities) {
				beforeDelete?.Invoke(entity);
				callbacks.ForEach(c => c.BeforeDelete(this, entity)); // notify before delete
				Session.Delete(entity);
			}
			Session.Flush(); // send commands to database
			foreach (var entity in entities) {
				callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
			}
			return entities.Count;
		}

		/// <summary>
		/// Create a sql query from query string and parameters
		/// </summary>
		private IQuery CreateSQLQuery(object query, object parameters) {
			var sqlQueryString = (string)query;
			var sqlParameters = (IDictionary<string, object>)parameters;
			IQuery sqlQuery = Session.CreateSQLQuery(sqlQueryString);
			foreach (var pair in sqlParameters) {
				sqlQuery = sqlQuery.SetParameter(pair.Key, pair.Value);
			}
			return sqlQuery;
		}

		/// <summary>
		/// Perform a raw update to database
		/// </summary>
		public long RawUpdate(object query, object parameters) {
			return CreateSQLQuery(query, parameters).ExecuteUpdate();
		}

		/// <summary>
		/// Perform a raw query to database
		/// </summary>
		public IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class {
			return CreateSQLQuery(query, parameters).Enumerable<T>();
		}
	}
}

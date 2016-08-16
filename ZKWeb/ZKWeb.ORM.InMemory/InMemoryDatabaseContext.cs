using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// InMemory database context
	/// </summary>
	internal class InMemoryDatabaseContext : IDatabaseContext {
		/// <summary>
		/// The database object
		/// </summary>
		private InMemoryDatabase Database { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public InMemoryDatabaseContext(InMemoryDatabase database) {
			Database = database;
		}

		/// <summary>
		/// Do Nothing
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// Do Nothing
		/// </summary>
		public void BeginTransaction(IsolationLevel? isolationLevel) { }

		/// <summary>
		/// Do Nothing
		/// </summary>
		public void FinishTransaction() { }

		/// <summary>
		/// Get the query object for specific entity
		/// </summary>
		public IQueryable<T> Query<T>()
			where T : class, IEntity {
			return Database.GetEntityStore(typeof(T)).Values.OfType<T>().AsQueryable();
		}

		/// <summary>
		/// Get single entity that matched the given predicate
		/// </summary>
		public T Get<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			return Query<T>().Where(predicate).FirstOrDefault();
		}

		/// <summary>
		/// Get how many entities that matched the given predicate
		/// </summary>
		public long Count<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			return Query<T>().Where(predicate).LongCount();
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
			var primaryKey = Database.EnsurePrimaryKey(entity);
			Database.GetEntityStore(typeof(T))[primaryKey] = entity;
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
			var primaryKey = Database.GetPrimaryKey(entity);
			if (primaryKey != null) {
				Database.GetEntityStore(typeof(T)).Remove(primaryKey);
			}
			callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
		}

		/// <summary>
		/// Batch save entities
		/// </summary>
		public void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
			where T : class, IEntity {
			entities = entities.Select(e => {
				Save(ref e, update);
				return e;
			}).ToList();
		}

		/// <summary>
		/// Batch update entities
		/// </summary>
		public long BatchUpdate<T>(Expression<Func<T, bool>> predicate, Action<T> update)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).ToList();
			var entitiesIterator = entities.AsEnumerable();
			BatchSave(ref entitiesIterator, update);
			return entities.Count();
		}

		/// <summary>
		/// Batch delete entities
		/// </summary>
		public long BatchDelete<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).ToList();
			entities.ForEach(e => Delete(e));
			return entities.Count;
		}

		/// <summary>
		/// Perform a raw update to database
		/// </summary>
		public long RawUpdate(object query, object parameters) {
			throw new NotSupportedException(
				"This operation is not supported for memory database");
		}

		/// <summary>
		/// Perform a raw query to database
		/// </summary>
		public IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class {
			throw new NotSupportedException(
				"This operation is not supported for memory database");
		}
	}
}

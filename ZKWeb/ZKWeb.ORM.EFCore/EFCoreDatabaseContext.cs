using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.FastReflection;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using ZKWeb.Database;
using ZKWebStandard.Utils;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Entity Framework Core database context
	/// </summary>
	public class EFCoreDatabaseContext : EFCoreDatabaseContextBase, IDatabaseContext {
		/// <summary>
		/// Entity Framework Core transaction
		/// </summary>
		private IDbContextTransaction Transaction { get; set; }
		/// <summary>
		/// Transaction level counter
		/// </summary>
		private int TransactionLevel;
		/// <summary>
		/// ORM name
		/// </summary>
		public string ORM { get { return ConstORM; } }
		internal const string ConstORM = "EFCore";
		/// <summary>
		/// Database type
		/// </summary>
		string IDatabaseContext.Database { get { return databaseType; } }
		private string databaseType;
		/// <summary>
		/// Underlying database connection
		/// </summary>
		public object DbConnection { get { return Database.GetDbConnection(); } }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public EFCoreDatabaseContext(string database, string connectionString)
			: base(database, connectionString) {
			Transaction = null;
			TransactionLevel = 0;
			databaseType = database;
		}

		/// <summary>
		/// Configure entity model
		/// </summary>
		/// <param name="modelBuilder">Model builder</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			// Call base method
			base.OnModelCreating(modelBuilder);
			// Register entity mappings
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider>();
			var entityTypes = providers.Select(p =>
				ReflectionUtils.GetGenericArguments(
				p.GetType(), typeof(IEntityMappingProvider<>))[0]).ToList();
			entityTypes.ForEach(t => Activator.CreateInstance(
				typeof(EFCoreEntityMappingBuilder<>).MakeGenericType(t), modelBuilder));
		}

		/// <summary>
		/// Dispose context and transaction
		/// </summary>
		public override void Dispose() {
			Transaction?.Dispose();
			Transaction = null;
			base.Dispose();
		}

		/// <summary>
		/// Begin a transaction
		/// </summary>
		public void BeginTransaction(IsolationLevel? isolationLevel = null) {
			var level = Interlocked.Increment(ref TransactionLevel);
			if (level == 1) {
				if (Transaction != null) {
					throw new InvalidOperationException("Transaction exists");
				}
				Transaction = (isolationLevel == null) ?
					Database.BeginTransaction() :
					Database.BeginTransaction(isolationLevel.Value);
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
			return Set<T>();
		}

		/// <summary>
		/// Get single entity that matched the given predicate
		/// It should return null if no matched entity found
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
		/// Insert or update entity
		/// </summary>
		private void InsertOrUpdate<T>(T entity, Action<T> update = null)
			where T : class, IEntity {
			var entityInfo = Entry(entity);
			update?.Invoke(entity);
			if (entityInfo.State == EntityState.Detached) {
				// It's not being tracked
				if (entityInfo.IsKeySet) {
					// The key is not default, we're not sure it's in database or not
					// check it first, it's rare so it shouldn't cause performance impact
					var property = typeof(T).FastGetProperty(nameof(IEntity<object>.Id));
					var id = property.FastGetValue(entity);
					var expr = ExpressionUtils.MakeMemberEqualiventExpression<T>(property.Name, id);
					if (Count(expr) > 0) {
						Update(entity);
					} else {
						Add(entity);
					}
				} else {
					// The key is default, set it's state to Added
					Add(entity);
				}
			} else {
				// It's being tracked, we don't need to attach it
			}
		}

		/// <summary>
		/// Save entity to database
		/// </summary>
		public void Save<T>(ref T entity, Action<T> update = null)
			where T : class, IEntity {
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			var entityLocal = entity; // can't use ref parameter in lambda
			callbacks.ForEach(c => c.BeforeSave(this, entityLocal)); // notify before save
			InsertOrUpdate(entityLocal, update);
			SaveChanges(); // send commands to database
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
			Remove(entity);
			SaveChanges(); // send commands to database
			callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
		}

		/// <summary>
		/// Batch save entities
		/// </summary>
		public void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
			where T : class, IEntity {
			var entitiesLocal = entities.ToList();
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			foreach (var entity in entitiesLocal) {
				callbacks.ForEach(c => c.BeforeSave(this, entity)); // notify before save
				InsertOrUpdate(entity, update);
			}
			SaveChanges(); // send commands to database
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
				Delete(entity);
			}
			SaveChanges(); // send commands to database
			foreach (var entity in entities) {
				callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
			}
			return entities.Count;
		}

		/// <summary>
		/// Batch save entities in faster way
		/// </summary>
		public void FastBatchSave<T, TPrimaryKey>(IEnumerable<T> entities)
			where T : class, IEntity<TPrimaryKey> {
			foreach (var entity in entities) {
				InsertOrUpdate(entity);
			}
			SaveChanges(); // send commands to database
		}

		/// <summary>
		/// Batch delete entities in faster way
		/// </summary>
		public long FastBatchDelete<T, TPrimaryKey>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity<TPrimaryKey>, new() {
			var entities = Query<T>().Where(predicate).Select(t => new T() { Id = t.Id });
			var count = entities.LongCount();
			RemoveRange(entities);
			SaveChanges(); // send commands to database
			return count;
		}

		/// <summary>
		/// Perform a raw update to database
		/// </summary>
		public long RawUpdate(object query, object parameters) {
			return Database.ExecuteSqlCommand((string)query, (object[])parameters);
		}

		/// <summary>
		/// Perform a raw query to database
		/// </summary>
		public IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class {
			return Set<T>().FromSql((string)query, (object[])parameters);
		}
	}
}

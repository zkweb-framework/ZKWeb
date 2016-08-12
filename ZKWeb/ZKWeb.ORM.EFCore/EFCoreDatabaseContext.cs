using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using ZKWeb.Database;
using ZKWeb.Server;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Entity Framework Core database context
	/// </summary>
	internal class EFCoreDatabaseContext : DbContext, IDatabaseContext {
		/// <summary>
		/// Database type
		/// </summary>
		private string DatabaseName { get; set; }
		/// <summary>
		/// Connection string
		/// </summary>
		private string ConnectionString { get; set; }
		/// <summary>
		/// Entity Framework Core transaction
		/// </summary>
		private IDbContextTransaction Transaction { get; set; }
		/// <summary>
		/// Transaction level counter
		/// </summary>
		private int TransactionLevel;

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public EFCoreDatabaseContext(string database, string connectionString) {
			DatabaseName = database;
			ConnectionString = connectionString;
			Transaction = null;
			TransactionLevel = 0;
		}

		/// <summary>
		/// Configure database context
		/// </summary>
		/// <param name="optionsBuilder">Options builder</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			// Set connection string
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			if (string.Compare(DatabaseName, "MSSQL", true) == 0) {
				optionsBuilder.UseSqlServer(ConnectionString);
			} else if (string.Compare(DatabaseName, "SQLite", true) == 0) {
				optionsBuilder.UseSqlite(
					ConnectionString.Replace("{{App_Data}}", pathConfig.AppDataDirectory));
			} else if (string.Compare(DatabaseName, "MySQL", true) == 0) {
				optionsBuilder.UseMySql(ConnectionString);
			} else if (string.Compare(DatabaseName, "InMemory", true) == 0) {
				optionsBuilder.UseInMemoryDatabase();
			} else {
				throw new ArgumentException($"unsupported database type {Database}");
			}
		}

		/// <summary>
		/// Configure entity model
		/// </summary>
		/// <param name="modelBuilder">Model builder</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);
			// register entity mappings
			// TODO
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
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <returns></returns>
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
		private void InsertOrUpdate<T>(T entity)
			where T : class, IEntity {
			var entityInfo = Entry(entity);
			if (entityInfo.State == EntityState.Detached) {
				// It's not being tracked
				if (entityInfo.IsKeySet) {
					// The key is not default, mark all properties as modified
					Update(entity);
				} else {
					// The key is default, set it's state to Added
					Add(entity);
				}
			} else {
				// It's being tracked, we don't need to do anything
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
			update?.Invoke(entityLocal);
			InsertOrUpdate(entityLocal);
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
			Delete(entity);
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
			entitiesLocal.ForEach(e =>
				callbacks.ForEach(c => c.BeforeSave(this, e))); // notify before save
			entitiesLocal.ForEach(e => {
				update?.Invoke(e);
				InsertOrUpdate(e);
			});
			SaveChanges(); // send commands to database
			entitiesLocal.ForEach(e =>
				callbacks.ForEach(c => c.AfterSave(this, e))); // notify after save
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
		public long BatchDelete<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).ToList();
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			entities.ForEach(e =>
				callbacks.ForEach(c => c.BeforeDelete(this, e))); // notify before delete
			entities.ForEach(e => Delete(e));
			SaveChanges(); // send commands to database
			entities.ForEach(e =>
				callbacks.ForEach(c => c.AfterDelete(this, e))); // notify after delete
			return entities.Count;
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
			where T : class, IEntity {
			return Query<T>().FromSql((string)query, (object[])parameters);
		}
	}
}

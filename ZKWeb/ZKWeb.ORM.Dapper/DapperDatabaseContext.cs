using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using ZKWeb.Database;
using ZKWeb.Server;
using Pomelo.Data.MySql;
using Npgsql;
using System;
using System.Threading;
using System.Linq;
using Dapper;
using System.Linq.Expressions;
using Dapper.Contrib.Extensions;
using System.FastReflection;
using System.Collections.Generic;
using ZKWeb.Storage;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Dapper database context
	/// </summary>
	internal class DapperDatabaseContext : IDatabaseContext {
		/// <summary>
		/// Dapper entity mappings
		/// </summary>
		private DapperEntityMappings Mappings { get; set; }
		/// <summary>
		/// Database connection
		/// </summary>
		private IDbConnection Connection { get; set; }
		/// <summary>
		/// Database transaction
		/// </summary>
		private IDbTransaction Transaction { get; set; }
		/// <summary>
		/// Transaction level counter
		/// </summary>
		private int TransactionLevel;
		/// <summary>
		/// ORM name
		/// </summary>
		public string ORM { get { return ConstORM; } }
		public const string ConstORM = "Dapper";
		/// <summary>
		/// Database type
		/// </summary>
		public string Database { get { return databaseType; } }
		private string databaseType;

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="mappings">Dapper entity mappings</param>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public DapperDatabaseContext(DapperEntityMappings mappings,
			string database, string connectionString) {
			Mappings = mappings;
			Connection = null;
			Transaction = null;
			TransactionLevel = 0;
			databaseType = database;
			// Create database connection
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			if (string.Compare(database, "MSSQL", true) == 0) {
				Connection = new SqlConnection(connectionString);
			} else if (string.Compare(database, "SQLite", true) == 0) {
				Connection = new SqliteConnection(
					connectionString.Replace("{{App_Data}}", pathConfig.AppDataDirectory));
			} else if (string.Compare(database, "MySQL", true) == 0) {
				Connection = new MySqlConnection(connectionString);
			} else if (string.Compare(database, "PostgreSQL", true) == 0) {
				Connection = new NpgsqlConnection(connectionString);
			} else {
				throw new ArgumentException($"unsupported database type {database}");
			}
			Connection.Open();
		}

		/// <summary>
		/// Finalize
		/// </summary>
		~DapperDatabaseContext() {
			Dispose();
		}

		/// <summary>
		/// Dispose connection and transaction
		/// </summary>
		public void Dispose() {
			Transaction?.Dispose();
			Transaction = null;
			Connection?.Dispose();
			Connection = null;
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
					Connection.BeginTransaction() :
					Connection.BeginTransaction(isolationLevel.Value);
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
		/// Attention: It's slow, you should use RawQuery
		/// </summary>
		public IQueryable<T> Query<T>()
			where T : class, IEntity {
			return Connection.GetAll<T>(Transaction).AsQueryable();
		}

		/// <summary>
		/// Get single entity that matched the given predicate
		/// It should return null if no matched entity found
		/// Attention: It's slow except predicate like x => x.Id == id
		/// </summary>
		public T Get<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			// If predicate is about compare primary key then we can use `Get` method
			if (predicate.Body is BinaryExpression) {
				var binaryExpr = (BinaryExpression)predicate.Body;
				if (binaryExpr.NodeType == ExpressionType.Equal &&
					binaryExpr.Left is MemberExpression &&
					((MemberExpression)binaryExpr.Left).Member.Name ==
					Mappings.GetMapping(typeof(T)).IdMember.Name &&
					binaryExpr.Right is ConstantExpression) {
					var primaryKey = ((ConstantExpression)binaryExpr.Right).Value;
					return Connection.Get<T>(primaryKey, Transaction);
				}
			}
			return Query<T>().FirstOrDefault(predicate);
		}

		/// <summary>
		/// Get how many entities that matched the given predicate
		/// Attention: It's slow, you should use RawQuery
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
			// If the primary key is empty, insert it
			var mapping = Mappings.GetMapping(typeof(T));
			var primaryKey = mapping.IdMember.FastGetValue(entity);
			if (primaryKey == null ||
				object.Equals(primaryKey, 0) ||
				object.Equals(primaryKey, -1) ||
				object.Equals(primaryKey, Guid.Empty)) {
				Connection.Insert(entity, Transaction);
			}
			// Try update first, if not exist then perform the insert
			if (!Connection.Update(entity, Transaction)) {
				Connection.Insert(entity, Transaction);
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
			Connection.Delete(entity, Transaction);
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
				update?.Invoke(entity);
				InsertOrUpdate(entity);
				callbacks.ForEach(c => c.AfterSave(this, entity)); // notify after save
			}
			entities = entitiesLocal;
		}

		/// <summary>
		/// Batch update entities
		/// Attention: It's slow, you should use RawUpdate
		/// </summary>
		public long BatchUpdate<T>(Expression<Func<T, bool>> predicate, Action<T> update)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).AsEnumerable();
			BatchSave(ref entities, update);
			return entities.LongCount();
		}

		/// <summary>
		/// Batch delete entities
		/// Attention: It's slow, you should use RawUpdate
		/// </summary>
		public long BatchDelete<T>(Expression<Func<T, bool>> predicate, Action<T> beforeDelete)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).ToList();
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			foreach (var entity in entities) {
				beforeDelete?.Invoke(entity);
				callbacks.ForEach(c => c.BeforeDelete(this, entity)); // notify before delete
				Connection.Delete(entity, Transaction);
				callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
			}
			return entities.Count;
		}

		/// <summary>
		/// Batch save entities in faster way
		/// Attention: It's still slow, you should use RawUpdate
		/// </summary>
		public void FastBatchSave<T, TPrimaryKey>(IEnumerable<T> entities)
			where T : class, IEntity<TPrimaryKey> {
			foreach (var entity in entities) {
				InsertOrUpdate(entity);
			}
		}

		/// <summary>
		/// Batch delete entities in faster way
		/// Attention: It's still slow, you should use RawUpdate
		/// </summary>
		public long FastBatchDelete<T, TPrimaryKey>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity<TPrimaryKey>, new() {
			var entities = Query<T>().Where(predicate);
			var count = 0L;
			foreach (var entity in entities) {
				Connection.Delete(entity, Transaction);
				++count;
			}
			return count;
		}

		/// <summary>
		/// Perform a raw update to database
		/// </summary>
		public long RawUpdate(object query, object parameters) {
			return Connection.Execute((string)query, parameters, Transaction);
		}

		/// <summary>
		/// Perform a raw query to database
		/// </summary>
		public IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class {
			return Connection.Query<T>((string)query, parameters, Transaction);
		}
	}
}

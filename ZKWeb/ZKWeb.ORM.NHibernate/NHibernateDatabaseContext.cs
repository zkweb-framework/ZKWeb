using DotLiquid;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using ZKWeb.Database;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// NHibernate database context<br/>
	/// Nhibernate的数据库上下文<br/>
	/// </summary>
	public class NHibernateDatabaseContext : IDatabaseContext {
		/// <summary>
		/// NHibernate session<br/>
		/// NHibernate的会话<br/>
		/// </summary>
		public ISession Session { get; protected set; }
		/// <summary>
		/// NHibernate transaction, maybe null if not used<br/>
		/// NHibernate的事务, 不使用时等于null<br/>
		/// </summary>
		public ITransaction Transaction { get; protected set; }
		/// <summary>
		/// Transaction level counter<br/>
		/// 事务嵌套计数<br/>
		/// </summary>
		protected int TransactionLevel;
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		public string ORM { get { return ConstORM; } }
#pragma warning disable CS1591
		public const string ConstORM = "NHibernate";
#pragma warning restore CS1591
		/// <summary>
		/// Database type<br/>
		/// 数据库类型<br/>
		/// </summary>
		public string Database { get { return databaseType; } }
#pragma warning disable CS1591
		protected string databaseType;
#pragma warning restore CS1591
		/// <summary>
		/// Underlying database connection<br/>
		/// 底层的数据库连接<br/>
		/// </summary>
		public object DbConnection { get { return Session.Connection; } }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="session">NHibernate session</param>
		/// <param name="database">Database type</param>
		public NHibernateDatabaseContext(ISession session, string database) {
			Session = session;
			Transaction = null;
			TransactionLevel = 0;
			databaseType = database;
		}

		/// <summary>
		/// Finalize<br/>
		/// 析构函数<br/>
		/// </summary>
		~NHibernateDatabaseContext() {
			Dispose();
		}

		/// <summary>
		/// Dispose nhibernate session and transaction<br/>
		/// 销毁NHibernate会话和事务<br/>
		/// </summary>
		public void Dispose() {
			Transaction?.Dispose();
			Transaction = null;
			Session?.Dispose();
			Session = null;
		}

		/// <summary>
		/// Begin a transaction<br/>
		/// 开始一个事务<br/>
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
		/// Finish the transaction<br/>
		/// 结束一个事务<br/>
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
		/// Get the query object for specific entity<br/>
		/// 获取指定实体类型的查询对象<br/>
		/// </summary>
		public IQueryable<T> Query<T>()
			where T : class, IEntity {
			return Session.Query<T>();
		}

		/// <summary>
		/// Get single entity that matched the given predicate<br/>
		/// 获取符合传入条件的单个实体<br/>
		/// </summary>
		public T Get<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			return Query<T>().FirstOrDefault(predicate);
		}

		/// <summary>
		/// Get how many entities that matched the given predicate<br/>
		/// 获取符合传入条件的实体数量<br/>
		/// </summary>
		public long Count<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			return Query<T>().LongCount(predicate);
		}

		/// <summary>
		/// Save entity to database<br/>
		/// 保存实体到数据库<br/>
		/// </summary>
		public void Save<T>(ref T entity, Action<T> update = null)
			where T : class, IEntity {
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			var entityLocal = entity; // can't use ref parameter in lambda
			callbacks.ForEach(c => c.BeforeSave(this, entityLocal)); // notify before save
			update?.Invoke(entityLocal);
			try {
				entityLocal = Session.Merge(entityLocal);
				Session.Flush(); // send commands to database
			} catch {
				Session.Evict(entityLocal); // remove entity from cache if any error
				throw;
			}
			callbacks.ForEach(c => c.AfterSave(this, entityLocal)); // notify after save
			entity = entityLocal;
		}

		/// <summary>
		/// Delete entity from database<br/>
		/// 删除数据库中的实体<br/>
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
		/// Batch save entities<br/>
		/// 批量保存实体<br/>
		/// </summary>
		public void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
			where T : class, IEntity {
			var entitiesLocal = entities.ToList();
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			try {
				for (int i = 0; i < entitiesLocal.Count; ++i) {
					var entity = entitiesLocal[i];
					callbacks.ForEach(c => c.BeforeSave(this, entity)); // notify before save
					update?.Invoke(entity);
					entity = Session.Merge(entity);
					entitiesLocal[i] = entity;
				}
				Session.Flush(); // send commands to database
			} catch {
				foreach (var entity in entitiesLocal) {
					Session.Evict(entity); // remove entities from cache if any error
				}
				throw;
			}
			foreach (var entity in entitiesLocal) {
				callbacks.ForEach(c => c.AfterSave(this, entity)); // notify after save
			}
			entities = entitiesLocal;
		}

		/// <summary>
		/// Batch update entities<br/>
		/// 批量更新实体<br/>
		/// </summary>
		public long BatchUpdate<T>(Expression<Func<T, bool>> predicate, Action<T> update)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).AsEnumerable();
			BatchSave(ref entities, update);
			return entities.LongCount();
		}

		/// <summary>
		/// Batch delete entities<br/>
		/// 批量删除实体<br/>
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
		/// Batch save entities in faster way<br/>
		/// 快速批量保存实体<br/>
		/// </summary>
		public void FastBatchSave<T, TPrimaryKey>(IEnumerable<T> entities)
			where T : class, IEntity<TPrimaryKey> {
			foreach (var entity in entities) {
				Session.Merge(entity);
			}
			Session.Flush(); // send commands to database
		}

		/// <summary>
		/// Batch delete entities in faster way<br/>
		/// 快速批量删除实体<br/>
		/// </summary>
		public long FastBatchDelete<T, TPrimaryKey>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity<TPrimaryKey>, new() {
			var entities = Query<T>().Where(predicate).Select(t => new T() { Id = t.Id });
			var count = 0L;
			foreach (var entity in entities) {
				Session.Delete(entity);
				++count;
			}
			Session.Flush(); // send commands to database
			return count;
		}

		/// <summary>
		/// Create a sql query from query string and parameters<br/>
		/// 根据查询字符串和参数创建SQL查询<br/>
		/// </summary>
		public IQuery CreateSQLQuery(object query, object parameters) {
			var sqlQueryString = (string)query;
			IQuery sqlQuery = Session.CreateSQLQuery(sqlQueryString);
			if (parameters != null && !(parameters is IDictionary<string, object>)) {
				parameters = Hash.FromAnonymousObject(parameters);
			}
			var sqlParameters = parameters as IDictionary<string, object>;
			if (sqlParameters != null) {
				foreach (var pair in sqlParameters) {
					if (pair.Value is IEnumerable && !(pair.Value is string)) {
						sqlQuery = sqlQuery.SetParameterList(pair.Key, (IEnumerable)pair.Value);
					} else {
						sqlQuery = sqlQuery.SetParameter(pair.Key, pair.Value);
					}
				}
			}
			return sqlQuery;
		}

		/// <summary>
		/// Perform a raw update to database<br/>
		/// 执行一个原生的更新操作<br/>
		/// </summary>
		public long RawUpdate(object query, object parameters) {
			return CreateSQLQuery(query, parameters).ExecuteUpdate();
		}

		/// <summary>
		/// Perform a raw query to database<br/>
		/// 执行一个原生的查询操作<br/>
		/// </summary>
		public IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class {
			return CreateSQLQuery(query, parameters)
				.SetResultTransformer(Transformers.AliasToBean<T>())
				.List<T>();
		}
	}
}

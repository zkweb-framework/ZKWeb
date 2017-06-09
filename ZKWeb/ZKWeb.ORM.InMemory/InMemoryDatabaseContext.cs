using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// InMemory database context<br/>
	/// 内存数据库的上下文<br/>
	/// </summary>
	public class InMemoryDatabaseContext : IDatabaseContext {
		/// <summary>
		/// The database object<br/>
		/// 数据库对象<br/>
		/// </summary>
		protected InMemoryDatabaseStore Store { get; set; }
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		public string ORM { get { return ConstORM; } }
#pragma warning disable CS1591
		public const string ConstORM = "InMemory";
#pragma warning restore CS1591
		/// <summary>
		/// Database type<br/>
		/// 数据库类型<br/>
		/// </summary>
		public string Database { get { return null; } }
		/// <summary>
		/// Underlying database connection<br/>
		/// 底层的数据库连接<br/>
		/// </summary>
		public object DbConnection { get { return null; } }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public InMemoryDatabaseContext(InMemoryDatabaseStore store) {
			Store = store;
		}

		/// <summary>
		/// Do Nothing<br/>
		/// 不做任何事情<br/>
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// Do Nothing<br/>
		/// 不做任何事情<br/>
		/// </summary>
		public void BeginTransaction(IsolationLevel? isolationLevel) { }

		/// <summary>
		/// Do Nothing<br/>
		/// 不做任何事情<br/>
		/// </summary>
		public void FinishTransaction() { }

		/// <summary>
		/// Get the query object for specific entity type<br/>
		/// 获取指定实体类型的查询对象<br/>
		/// </summary>
		public IQueryable<T> Query<T>()
			where T : class, IEntity {
			return Store.GetEntityStore(typeof(T)).Values.OfType<T>().AsQueryable();
		}

		/// <summary>
		/// Get single entity that matched the given predicate<br/>
		/// 获取符合传入条件的单个实体<br/>
		/// </summary>
		public T Get<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			return Query<T>().Where(predicate).FirstOrDefault();
		}

		/// <summary>
		/// Get how many entities that matched the given predicate<br/>
		/// 获取符合传入条件的实体数量<br/>
		/// </summary>
		public long Count<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity {
			return Query<T>().Where(predicate).LongCount();
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
			var primaryKey = Store.EnsurePrimaryKey(entity);
			Store.GetEntityStore(typeof(T))[primaryKey] = entity;
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
			var primaryKey = Store.GetPrimaryKey(entity);
			if (primaryKey != null) {
				Store.GetEntityStore(typeof(T)).Remove(primaryKey);
			}
			callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
		}

		/// <summary>
		/// Batch save entities<br/>
		/// 批量保存实体<br/>
		/// </summary>
		public void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
			where T : class, IEntity {
			entities = entities.Select(e => {
				Save(ref e, update);
				return e;
			}).ToList();
		}

		/// <summary>
		/// Batch update entities<br/>
		/// 批量更新实体<br/>
		/// </summary>
		public long BatchUpdate<T>(Expression<Func<T, bool>> predicate, Action<T> update)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).ToList();
			var entitiesIterator = entities.AsEnumerable();
			BatchSave(ref entitiesIterator, update);
			return entities.Count();
		}

		/// <summary>
		/// Batch delete entities<br/>
		/// 批量删除实体<br/>
		/// </summary>
		public long BatchDelete<T>(Expression<Func<T, bool>> predicate, Action<T> beforeDelete)
			where T : class, IEntity {
			var entities = Query<T>().Where(predicate).ToList();
			entities.ForEach(e => { beforeDelete?.Invoke(e); Delete(e); });
			return entities.Count;
		}

		/// <summary>
		/// Batch save entities in faster way<br/>
		/// 快速批量保存实体<br/>
		/// </summary>
		public void FastBatchSave<T, TPrimaryKey>(IEnumerable<T> entities)
			where T : class, IEntity<TPrimaryKey> {
			var store = Store.GetEntityStore(typeof(T));
			foreach (var entity in entities) {
				var primaryKey = Store.EnsurePrimaryKey(entity);
				store[primaryKey] = entity;
			}
		}

		/// <summary>
		/// Batch delete entities in faster way<br/>
		/// 快速批量删除实体<br/>
		/// </summary>
		public long FastBatchDelete<T, TPrimaryKey>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity<TPrimaryKey>, new() {
			var entities = Query<T>().Where(predicate);
			var store = Store.GetEntityStore(typeof(T));
			var count = 0L;
			foreach (var entity in entities) {
				store.Remove(entity.Id);
				++count;
			}
			return count;
		}

		/// <summary>
		/// Perform a raw update to database<br/>
		/// 执行一个原生的更新操作<br/>
		/// </summary>
		public long RawUpdate(object query, object parameters) {
			throw new NotSupportedException(
				"This operation is not supported for memory database");
		}

		/// <summary>
		/// Perform a raw query to database<br/>
		/// 执行一个原生的查询操作<br/>
		/// </summary>
		public IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class {
			throw new NotSupportedException(
				"This operation is not supported for memory database");
		}
	}
}

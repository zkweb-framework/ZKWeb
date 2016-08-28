using System.Data;
using ZKWeb.Database;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.FastReflection;
using System.Collections.Generic;
using MongoDB.Driver;
using ZKWebStandard.Utils;
using MongoDB.Bson;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// Dapper database context
	/// </summary>
	internal class MongoDBDatabaseContext : IDatabaseContext {
		/// <summary>
		/// MongoDB entity mappings
		/// </summary>
		private MongoDBEntityMappings Mappings { get; set; }
		/// <summary>
		/// Database object
		/// </summary>
		private IMongoDatabase Database { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="connectionUrl">Connection url</param>
		/// <param name="mappings">Dapper entity mappings</param>
		public MongoDBDatabaseContext(
			MongoUrl connectionUrl,
			MongoDBEntityMappings mappings) {
			Mappings = mappings;
			Database = new MongoClient(connectionUrl).GetDatabase(connectionUrl.DatabaseName);
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// Do Nothing
		/// </summary>
		public void BeginTransaction(IsolationLevel? isolationLevel = null) { }

		/// <summary>
		/// Do Nothing
		/// </summary>
		public void FinishTransaction() { }

		/// <summary>
		/// Get mongo collection
		/// </summary>
		private IMongoCollection<T> GetCollection<T>()
			where T : class {
			var mapping = Mappings.GetMapping(typeof(T));
			return Database.GetCollection<T>(mapping.CollectionName);
		}

		/// <summary>
		/// Get the query object for specific entity
		/// </summary>
		public IQueryable<T> Query<T>()
			where T : class, IEntity {
			return GetCollection<T>().AsQueryable();
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
		/// Make expression for filter entity by id
		/// Result is (e => e.Id == entity.Id)
		/// </summary>
		private Expression<Func<T, bool>> MakeIdExpression<T>(T entity) {
			var mapping = Mappings.GetMapping(typeof(T));
			return ExpressionUtils.MakeMemberEqualiventExpression<T>(
				mapping.IdMember.Name, mapping.IdMember.FastGetValue(entity));
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
			GetCollection<T>().ReplaceOne(
				MakeIdExpression(entity), entity,
				new UpdateOptions() { IsUpsert = true });
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
			GetCollection<T>().DeleteOne(MakeIdExpression(entity));
			callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
		}

		/// <summary>
		/// Batch save entities
		/// </summary>
		public void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
			where T : class, IEntity {
			var entitiesLocal = entities.ToList();
			var callbacks = Application.Ioc.ResolveMany<IEntityOperationHandler<T>>().ToList();
			var collection = GetCollection<T>();
			foreach (var entity in entitiesLocal) {
				callbacks.ForEach(c => c.BeforeSave(this, entity)); // notify before save
				update?.Invoke(entity);
				collection.ReplaceOne(
					MakeIdExpression(entity), entity,
					new UpdateOptions() { IsUpsert = true });
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
			var collection = GetCollection<T>();
			foreach (var entity in entities) {
				beforeDelete?.Invoke(entity);
				callbacks.ForEach(c => c.BeforeDelete(this, entity)); // notify before delete
				collection.DeleteOne(MakeIdExpression(entity));
				callbacks.ForEach(c => c.AfterDelete(this, entity)); // notify after delete
			}
			return entities.Count;
		}

		/// <summary>
		/// Perform a raw update to database
		/// </summary>
		public long RawUpdate(object query, object parameters) {
			if (query is Command<int>) {
				return Database.RunCommand(
					(Command<int>)query, parameters as ReadPreference);
			} else if (query is Command<long>) {
				return Database.RunCommand(
					(Command<long>)query, parameters as ReadPreference);
			} else if (query is Func<IMongoDatabase, int>) {
				return ((Func<IMongoDatabase, int>)query).Invoke(Database);
			} else if (query is Func<IMongoDatabase, long>) {
				return ((Func<IMongoDatabase, long>)query).Invoke(Database);
			}
			throw new ArgumentException(
				"Unsupported query type, you can use Command<int> or Func<IMongoDatabase, int>");
		}

		/// <summary>
		/// Perform a raw query to database
		/// </summary>
		public IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class {
			if (query is FilterDefinition<T>) {
				return GetCollection<T>().Find(
					(FilterDefinition<T>)query,
					parameters as FindOptions).ToEnumerable();
			} else if (query is Command<T>) {
				return new[] { Database.RunCommand(
					(Command<T>)query, parameters as ReadPreference)
				};
			} else if (query is BsonJavaScript[]) {
				var scripts = (BsonJavaScript[])query;
				return GetCollection<T>().MapReduce(
					scripts[0], scripts[1],
					parameters as MapReduceOptions<T, T>).ToEnumerable();
			} else {
				throw new ArgumentException(
					"Unsupported query type, you can use FilterDefinition<T> or " +
					"Command<T> or BsonJavaScript[2] (for MapReduce)");
			}
		}
	}
}

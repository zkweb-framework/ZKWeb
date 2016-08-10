using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ZKWeb.Database;
using ZKWebStandard.Utils;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// A simple memory database
	/// The performance is poor, it should only use for testing
	/// </summary>
	public class InMemoryDatabase {
		/// <summary>
		/// Data store
		/// { Type: { key: object } }
		/// </summary>
		protected ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>
			Store { get; set; }
		/// <summary>
		/// The sequence for integer type primary key
		/// { Type: value }
		/// </summary>
		protected ConcurrentDictionary<Type, long> PrimaryKeySequence { get; set; }

		/// <summary>
		/// Get data store for specified type
		/// </summary>
		/// <param name="entityType">Entity type</param>
		/// <returns></returns>
		public IDictionary<object, object> GetEntityStore(Type entityType) {
			return Store.GetOrAdd(entityType, t => new ConcurrentDictionary<object, object>());
		}
		
		public object GetPrimaryKey<T>(T entity) {
			throw new NotImplementedException();
		}
		
		public object EnsurePrimaryKey<T>(T entity) {
			if (entity is IEntity<int>) {
				var typedEntity = (IEntity<int>)entity;
				if (typedEntity.Id == 0) {
					var id = PrimaryKeySequence.GetOrAdd(typeof(T), 0);
					
				}
				return typedEntity.Id;
			} else if (entity is IEntity<long>) {
				var typedEntity = (IEntity<long>)entity;
				if (typedEntity.Id == 0) {
					var id = PrimaryKeySequence.GetOrAdd(typeof(T), 0);

				}
				return typedEntity.Id;
			} else if (entity is IEntity<Guid>) {
				var typedEntity = (IEntity<Guid>)entity;
				if (typedEntity.Id == Guid.Empty) {
					typedEntity.Id = GuidUtils.SequentialGuid(DateTime.UtcNow);
				}
				return typedEntity.Id;
			}
			return GetPrimaryKey(entity);
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using ZKWeb.Database;
using ZKWebStandard.Utils;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// A simple memory database store<br/>
	/// It shouldn't use in production environment,<br/>
	/// The performance will be very poor<br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// </summary>
	internal class InMemoryDatabaseStore {
		/// <summary>
		/// Data store<br/>
		/// <br/>
		/// { Type: { key: object } }
		/// </summary>
		private ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>
			Store { get; set; }
		/// <summary>
		/// Type to mapping definition<br/>
		/// <br/>
		/// </summary>
		private ConcurrentDictionary<Type, IInMemoryEntityMapping> Mappings { get; set; }
		/// <summary>
		/// Type to primary key sequence, only for integer type<br/>
		/// <br/>
		/// </summary>
		private ConcurrentDictionary<Type, long> PrimaryKeySequence { get; set; }
		/// <summary>
		/// The lock for the sequence increment<br/>
		/// <br/>
		/// </summary>
		private object PrimaryKeySequenceLock { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		public InMemoryDatabaseStore() {
			Store = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();
			Mappings = new ConcurrentDictionary<Type, IInMemoryEntityMapping>();
			PrimaryKeySequence = new ConcurrentDictionary<Type, long>();
			PrimaryKeySequenceLock = new object();
			// Build entity mappings
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider>();
			var groupedProviders = providers.GroupBy(p =>
				ReflectionUtils.GetGenericArguments(
				p.GetType(), typeof(IEntityMappingProvider<>))[0]);
			foreach (var group in groupedProviders) {
				var builder = (IInMemoryEntityMapping)Activator.CreateInstance(
					typeof(InMemoryEntityMappingBuilder<>).MakeGenericType(group.Key));
				Mappings[group.Key] = builder;
			}
		}

		/// <summary>
		/// Get data store for specified type<br/>
		/// <br/>
		/// </summary>
		/// <param name="entityType">Entity type</param>
		/// <returns></returns>
		public IDictionary<object, object> GetEntityStore(Type entityType) {
			return Store.GetOrAdd(entityType, t => new ConcurrentDictionary<object, object>());
		}

		/// <summary>
		/// Get the primary key object from entity<br/>
		/// <br/>
		/// </summary>
		public object GetPrimaryKey<T>(T entity) {
			var mapping = Mappings[typeof(T)];
			return mapping.IdMember.FastGetValue(entity);
		}

		/// <summary>
		/// If an entity have a integer or guid primary key, and it's empty,<br/>
		/// then generate a new primary key for it.<br/>
		/// Return the final primary key.<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public object EnsurePrimaryKey<T>(T entity) {
			var mapping = Mappings[typeof(T)];
			var primaryKey = GetPrimaryKey(entity);
			if ((primaryKey is int || primaryKey is long) && ((long)primaryKey) <= 0) {
				lock (PrimaryKeySequenceLock) {
					primaryKey = PrimaryKeySequence.GetOrAdd(typeof(T), 0) + 1;
					PrimaryKeySequence[typeof(T)] = (long)primaryKey;
				}
				mapping.IdMember.FastSetValue(entity, primaryKey);
			} else if (primaryKey is Guid && (Guid)primaryKey == Guid.Empty) {
				primaryKey = GuidUtils.SequentialGuid(DateTime.UtcNow);
				mapping.IdMember.FastSetValue(entity, primaryKey);
			}
			return primaryKey;
		}
	}
}

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
	/// 一个简单的内存数据库储存<br/>
	/// 它不应该用在生产环境<br/>
	/// 性能会很差<br/>
	/// </summary>
	public class InMemoryDatabaseStore {
		/// <summary>
		/// Data store<br/>
		/// 数据储存<br/>
		/// { Type: { key: object } }
		/// </summary>
		protected ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>
			Store { get; set; }
		/// <summary>
		/// Type to mapping definition<br/>
		/// 类型到映射的定义<br/>
		/// </summary>
		protected ConcurrentDictionary<Type, IInMemoryEntityMapping> Mappings { get; set; }
		/// <summary>
		/// Type to primary key sequence, only for integer type<br/>
		/// 类型到主键的序号, 只供int主键使用<br/>
		/// </summary>
		protected ConcurrentDictionary<Type, long> PrimaryKeySequence { get; set; }
		/// <summary>
		/// The lock for the sequence increment<br/>
		/// 增加序号时使用的锁<br/>
		/// </summary>
		protected object PrimaryKeySequenceLock { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public InMemoryDatabaseStore(
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			Store = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();
			Mappings = new ConcurrentDictionary<Type, IInMemoryEntityMapping>();
			PrimaryKeySequence = new ConcurrentDictionary<Type, long>();
			PrimaryKeySequenceLock = new object();
			// Build entity mappings
			var entityProviders = providers
				.GroupBy(p => ReflectionUtils.GetGenericArguments(
					p.GetType(), typeof(IEntityMappingProvider<>))[0])
				.ToList();
			foreach (var group in entityProviders) {
				var builder = Activator.CreateInstance(
					typeof(InMemoryEntityMappingBuilder<>).MakeGenericType(group.Key),
					handlers, group.AsEnumerable());
				Mappings[group.Key] = (IInMemoryEntityMapping)builder;
			}
		}

		/// <summary>
		/// Get data store for specified type<br/>
		/// 获取类型对应的数据储存<br/>
		/// </summary>
		/// <param name="entityType">Entity type</param>
		/// <returns></returns>
		public IDictionary<object, object> GetEntityStore(Type entityType) {
			return Store.GetOrAdd(entityType, t => new ConcurrentDictionary<object, object>());
		}

		/// <summary>
		/// Get the primary key object from entity<br/>
		/// 获取实体的主键对象<br/>
		/// </summary>
		public object GetPrimaryKey<T>(T entity) {
			var mapping = Mappings[typeof(T)];
			return mapping.IdMember.FastGetValue(entity);
		}

		/// <summary>
		/// If an entity have a integer or guid primary key, and it's empty,<br/>
		/// then generate a new primary key for it.<br/>
		/// Return the final primary key.<br/>
		/// 如果实体有一个数值或者guid主键, 并且主键为空<br/>
		/// 则生成一个新的主键<br/>
		/// 返回最终的主键<br/>
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

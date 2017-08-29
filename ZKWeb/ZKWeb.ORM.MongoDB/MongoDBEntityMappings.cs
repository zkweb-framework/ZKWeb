using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ZKWeb.Database;
using ZKWebStandard.Utils;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// MongoDB entity mappings<br/>
	/// MongoDB的实体映射集合<br/>
	/// </summary>
	public class MongoDBEntityMappings {
		/// <summary>
		/// Type to mapping definition<br/>
		/// 类型到映射的定义<br/>
		/// </summary>
		protected ConcurrentDictionary<Type, IMongoDBEntityMapping> Mappings { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public MongoDBEntityMappings(
			MongoUrl connectionUrl,
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			Mappings = new ConcurrentDictionary<Type, IMongoDBEntityMapping>();
			// Build entity mappings
			var entityProviders = providers
				.GroupBy(p => ReflectionUtils.GetGenericArguments(
					p.GetType(), typeof(IEntityMappingProvider<>))[0])
				.ToList();
			var client = new MongoClient(connectionUrl);
			var database = client.GetDatabase(connectionUrl.DatabaseName);
			foreach (var group in entityProviders) {
				var builder = Activator.CreateInstance(
					typeof(MongoDBEntityMappingBuilder<>).MakeGenericType(group.Key),
					database, handlers, group.AsEnumerable());
				Mappings[group.Key] = (IMongoDBEntityMapping)builder;
			}
		}

		/// <summary>
		/// Get mapping for entity type<br/>
		/// 获取实体类型对应的映射<br/>
		/// </summary>
		/// <param name="type">Entity type</param>
		/// <returns></returns>
		public IMongoDBEntityMapping GetMapping(Type type) {
			return Mappings[type];
		}
	}
}

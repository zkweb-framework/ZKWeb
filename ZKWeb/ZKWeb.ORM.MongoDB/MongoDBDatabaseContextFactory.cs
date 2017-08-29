using MongoDB.Driver;
using System;
using System.Collections.Generic;
using ZKWeb.Database;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// MongoDB database context factory<br/>
	/// MongoDB doesn't need to migrate database scheme<br/>
	/// MongoDB的数据库上下文生成器<br/>
	/// MongoDB不需要迁移数据库<br/>
	/// </summary>
	public class MongoDBDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Connection url<br/>
		/// 连接字符串<br/>
		/// </summary>
		protected MongoUrl ConnectionUrl { get; set; }
		/// <summary>
		/// MongoDB entity mappings<br/>
		/// MongoDB的实体映射<br/>
		/// </summary>
		protected MongoDBEntityMappings Mappings { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public MongoDBDatabaseContextFactory(string database, string connectionString) :
			this(database, connectionString,
				Application.Ioc.ResolveMany<IDatabaseInitializeHandler>(),
				Application.Ioc.ResolveMany<IEntityMappingProvider>()) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public MongoDBDatabaseContextFactory(
			string database, string connectionString,
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			if (string.Compare(database, "MongoDB", true) != 0) {
				throw new ArgumentException($"Database type should be MongoDB");
			}
			ConnectionUrl = new MongoUrl(connectionString);
			if (string.IsNullOrEmpty(ConnectionUrl.DatabaseName)) {
				throw new ArgumentException("Please set the database name in connection string");
			}
			Mappings = new MongoDBEntityMappings(ConnectionUrl, handlers, providers);
		}

		/// <summary>
		/// Create database context<br/>
		/// 创建数据库上下文<br/>
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new MongoDBDatabaseContext(ConnectionUrl, Mappings);
		}
	}
}

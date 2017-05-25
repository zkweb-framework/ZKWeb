using MongoDB.Driver;
using System;
using ZKWeb.Database;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// MongoDB database context factory<br/>
	/// MongoDB doesn't need to migrate database scheme<br/>
	/// MongoDB的数据库上下文生成器<br/>
	/// MongoDB不需要迁移数据库<br/>
	/// </summary>
	internal class MongoDBDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Connection url<br/>
		/// 连接字符串<br/>
		/// </summary>
		private MongoUrl ConnectionUrl { get; set; }
		/// <summary>
		/// MongoDB entity mappings<br/>
		/// MongoDB的实体映射<br/>
		/// </summary>
		private MongoDBEntityMappings Mappings { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public MongoDBDatabaseContextFactory(string database, string connectionString) {
			if (string.Compare(database, "MongoDB", true) != 0) {
				throw new ArgumentException($"Database type should be MongoDB");
			}
			ConnectionUrl = new MongoUrl(connectionString);
			if (string.IsNullOrEmpty(ConnectionUrl.DatabaseName)) {
				throw new ArgumentException("Please set the database name in connection string");
			}
			Mappings = new MongoDBEntityMappings(ConnectionUrl);
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

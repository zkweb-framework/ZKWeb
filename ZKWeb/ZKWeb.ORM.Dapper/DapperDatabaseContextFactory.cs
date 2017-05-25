using ZKWeb.Database;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Dapper database context factory<br/>
	/// Attention:<br/>
	/// There no database migration support for dapper,<br/>
	/// you should create the database scheme manually or use other ORM as a bootstrap<br/>
	/// And because dapper not support relational mapping at all, you need explicit create join tables<br/>
	/// Dapper的数据库上下文生成器<br/>
	/// 注意:<br/>
	/// Dapper不支持数据库迁移<br/>
	/// 你应该手动创建好数据库表, 或者先用其他ORM创建<br/>
	/// 并且因为Dapper不支持关系映射, 你应该明确的创建join使用的表<br/>
	/// </summary>
	internal class DapperDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Dapper entity mappings<br/>
		/// Dapper的实体映射<br/>
		/// </summary>
		private DapperEntityMappings Mappings { get; set; }
		/// <summary>
		/// Database type<br/>
		/// 数据库类型<br/>
		/// </summary>
		public string Database { get; set; }
		/// <summary>
		/// Connection string<br/>
		/// 连接字符串<br/>
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public DapperDatabaseContextFactory(string database, string connectionString) {
			Mappings = new DapperEntityMappings();
			Database = database;
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Create a database context<br/>
		/// 创建数据库上下文<br/>
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new DapperDatabaseContext(Mappings, Database, ConnectionString);
		}
	}
}

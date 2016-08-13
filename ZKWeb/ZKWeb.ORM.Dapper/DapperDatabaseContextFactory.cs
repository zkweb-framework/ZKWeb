using ZKWeb.Database;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Dapper database context factory
	/// Attention:
	/// There no database migration support for dapper,
	/// you should create the database scheme manually or use other ORM as a bootstrap
	/// And because dapper not support relational mapping at all, you need explicit create join tables
	/// </summary>
	internal class DapperDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Dapper entity mappings
		/// </summary>
		private DapperEntityMappings Mappings { get; set; }
		/// <summary>
		/// Database type
		/// </summary>
		public string Database { get; set; }
		/// <summary>
		/// Connection string
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public DapperDatabaseContextFactory(string database, string connectionString) {
			Mappings = new DapperEntityMappings();
			Database = database;
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Create a database context
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new DapperDatabaseContext(Mappings, Database, ConnectionString);
		}
	}
}

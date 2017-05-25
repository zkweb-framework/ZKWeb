using ZKWeb.Database;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Dapper database context factory<br/>
	/// Attention:<br/>
	/// There no database migration support for dapper,<br/>
	/// you should create the database scheme manually or use other ORM as a bootstrap<br/>
	/// And because dapper not support relational mapping at all, you need explicit create join tables<br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// </summary>
	internal class DapperDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Dapper entity mappings<br/>
		/// <br/>
		/// </summary>
		private DapperEntityMappings Mappings { get; set; }
		/// <summary>
		/// Database type<br/>
		/// <br/>
		/// </summary>
		public string Database { get; set; }
		/// <summary>
		/// Connection string<br/>
		/// <br/>
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
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
		/// <br/>
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new DapperDatabaseContext(Mappings, Database, ConnectionString);
		}
	}
}

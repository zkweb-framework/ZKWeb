using ZKWeb.Database;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Entity Framework Core database context factory
	/// </summary>
	internal class EFCoreDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Database type
		/// </summary>
		private string Database { get; set; }
		/// <summary>
		/// Connection string
		/// </summary>
		private string ConnectionString { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public EFCoreDatabaseContextFactory(string database, string connectionString) {
			Database = database;
			ConnectionString = connectionString;
			// initialize database scheme
			// TODO
		}

		/// <summary>
		/// Create database context
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new EFCoreDatabaseContext(Database, ConnectionString);
		}
	}
}

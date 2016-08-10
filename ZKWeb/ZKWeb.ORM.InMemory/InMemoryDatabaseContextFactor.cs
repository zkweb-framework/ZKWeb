using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// InMemory database context factor
	/// This factor hold a memory database for all context it created
	/// </summary>
	internal class InMemoryDatabaseContextFactor : IDatabaseContextFactor {
		/// <summary>
		/// The database object
		/// </summary>
		private InMemoryDatabase Database { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Not using</param>
		/// <param name="connectionString">Not using</param>
		public InMemoryDatabaseContextFactor(string database, string connectionString) {
			Database = new InMemoryDatabase();
		}

		/// <summary>
		/// Create a database context
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new InMemoryDatabaseContext(Database);
		}
	}
}

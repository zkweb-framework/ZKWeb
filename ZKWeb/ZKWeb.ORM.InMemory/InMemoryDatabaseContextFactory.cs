using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// InMemory database context factory
	/// This factory hold a memory database for all context it created
	/// </summary>
	internal class InMemoryDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// The store object
		/// </summary>
		private InMemoryDatabaseStore Store { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Not using</param>
		/// <param name="connectionString">Not using</param>
		public InMemoryDatabaseContextFactory(string database, string connectionString) {
			Store = new InMemoryDatabaseStore();
		}

		/// <summary>
		/// Create a database context
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new InMemoryDatabaseContext(Store);
		}
	}
}

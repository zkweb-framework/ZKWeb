using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// InMemory database context factory<br/>
	/// This factory hold a memory database for all context it created<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	internal class InMemoryDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// The store object<br/>
		/// <br/>
		/// </summary>
		private InMemoryDatabaseStore Store { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="database">Not using</param>
		/// <param name="connectionString">Not using</param>
		public InMemoryDatabaseContextFactory(string database, string connectionString) {
			Store = new InMemoryDatabaseStore();
		}

		/// <summary>
		/// Create a database context<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new InMemoryDatabaseContext(Store);
		}
	}
}

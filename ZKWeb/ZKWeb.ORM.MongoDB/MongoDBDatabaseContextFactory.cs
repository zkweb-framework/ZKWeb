using System;
using ZKWeb.Database;

namespace ZKWeb.ORM.MongoDB {
	/// <summary>
	/// MongoDB database context factory
	/// MongoDB doesn't need to migrate database scheme
	/// </summary>
	public class MongoDBDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Connection string
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public MongoDBDatabaseContextFactory(string database, string connectionString) {
			if (string.Compare(database, "MongoDB", true) != 0) {
				throw new ArgumentException($"Database type should be MongoDB");
			}
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Create database context
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			throw new NotImplementedException();
		}
	}
}

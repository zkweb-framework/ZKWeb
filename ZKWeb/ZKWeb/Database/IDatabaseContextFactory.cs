namespace ZKWeb.Database {
	/// <summary>
	/// Interface for database context factory
	/// It should initialize database scheme when construct
	/// </summary>
	public interface IDatabaseContextFactory {
		/// <summary>
		/// Create database context
		/// </summary>
		/// <returns></returns>
		IDatabaseContext CreateContext();
	}
}

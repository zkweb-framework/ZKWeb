namespace ZKWeb.Database {
	/// <summary>
	/// Interface for database context factor
	/// It should initialize database scheme when construct
	/// </summary>
	public interface IDatabaseContextFactor {
		/// <summary>
		/// Create database context
		/// </summary>
		/// <returns></returns>
		IDatabaseContext CreateContext();
	}
}

namespace ZKWeb.Database {
	/// <summary>
	/// Interface provide some extension during database initializion
	/// </summary>
	public interface IDatabaseInitializeHandler {
		/// <summary>
		/// Convert table name
		/// Attention: Not all ORM support this feature
		/// </summary>
		/// <param name="tableName">The table name</param>
		void ConvertTableName(ref string tableName);
	}
}

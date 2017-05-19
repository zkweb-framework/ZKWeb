namespace ZKWeb.Database {
	/// <summary>
	/// Interface for database context factory<br/>
	/// 数据库上下文生成器的接口<br/>
	/// </summary>
	/// <seealso cref="IDatabaseContext"/>
	/// <seealso cref="DatabaseManager"/>
	public interface IDatabaseContextFactory {
		/// <summary>
		/// Create database context<br/>
		/// 创建数据库上下文<br/>
		/// </summary>
		/// <returns></returns>
		IDatabaseContext CreateContext();
	}
}

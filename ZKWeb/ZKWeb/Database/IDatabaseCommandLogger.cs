namespace ZKWeb.Database {
	/// <summary>
	/// Interface for logging database command such as sql query<br/>
	/// 用于记录数据库命令(例如sql)的记录器接口
	/// </summary>
	public interface IDatabaseCommandLogger {
		/// <summary>
		/// Log database command<br/>
		/// 记录数据库命令<br/>
		/// </summary>
		/// <param name="context">Which context execute this command</param>
		/// <param name="command">What command executed</param>
		/// <param name="metadata">Different metadata for each ORM</param>
		void LogCommand(IDatabaseContext context, string command, object metadata);
	}
}

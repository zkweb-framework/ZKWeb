namespace ZKWeb.Database {
	/// <summary>
	/// Interface used to customize the processing of the database initialization<br/>
	/// 用于自定义数据库初始化时的处理的接口<br/>
	/// </summary>
	/// <example>
	/// <code lanuage="cs">
	/// [ExportMany]
	/// public class DatabaseInitializeHandler : IDatabaseInitializeHandler {
	///		public void ConvertTableName(ref string tableName) {
	///			tableName = "ZKWeb_" + tableName;
	///		}
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="DatabaseManager"/>
	/// <seealso cref="IDatabaseContextFactory"/>
	public interface IDatabaseInitializeHandler {
		/// <summary>
		/// Convert table name<br/>
		/// Attention: Not all ORMs support this feature<br/>
		/// 转换表名称<br/>
		/// 注意: 不是所有的ORM都支持这项功能<br/>
		/// </summary>
		/// <param name="tableName">The table name</param>
		void ConvertTableName(ref string tableName);
	}
}

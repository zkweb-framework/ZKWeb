using NHibernate.Dialect;
using System.Data;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Better pgsql dialect<br/>
	/// What's improved<br/>
	/// - Support Guid type<br/>
	/// 更好的pgsql配置<br/>
	/// 改进点<br/>
	/// - 支持Guid类型<br/>
	/// </summary>
	public class BetterPostgreSQLDialect : PostgreSQLDialect {
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public BetterPostgreSQLDialect() {
			RegisterColumnType(DbType.Guid, "uuid");
		}
	}
}

using NHibernate.Dialect;
using System.Data;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Better pgsql dialect
	/// What's improved
	/// - Support Guid type
	/// </summary>
	internal class BetterPostgreSQLDialect : PostgreSQLDialect {
		/// <summary>
		/// Initialize
		/// </summary>
		public BetterPostgreSQLDialect() {
			RegisterColumnType(DbType.Guid, "uuid");
		}
	}
}

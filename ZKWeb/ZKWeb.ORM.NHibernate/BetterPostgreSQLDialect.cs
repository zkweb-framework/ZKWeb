using NHibernate.Dialect;
using System.Data;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Better pgsql dialect<br/>
	/// What's improved<br/>
	/// - Support Guid type<br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// </summary>
	internal class BetterPostgreSQLDialect : PostgreSQLDialect {
		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		public BetterPostgreSQLDialect() {
			RegisterColumnType(DbType.Guid, "uuid");
		}
	}
}

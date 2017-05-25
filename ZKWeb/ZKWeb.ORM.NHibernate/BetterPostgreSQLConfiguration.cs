using FluentNHibernate.Cfg.Db;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Better postgre sql configuration<br/>
	/// <br/>
	/// </summary>
	internal class BetterPostgreSQLConfiguration : PostgreSQLConfiguration {
		/// <summary>
		/// Better configuration<br/>
		/// <br/>
		/// </summary>
		public static PostgreSQLConfiguration Better {
			get { return new BetterPostgreSQLConfiguration().Dialect<BetterPostgreSQLDialect>(); }
		}
	}
}

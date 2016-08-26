using FluentNHibernate.Cfg.Db;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Better postgre sql configuration
	/// </summary>
	internal class BetterPostgreSQLConfiguration : PostgreSQLConfiguration {
		/// <summary>
		/// Better configuration
		/// </summary>
		public static PostgreSQLConfiguration Better {
			get { return new BetterPostgreSQLConfiguration().Dialect<BetterPostgreSQLDialect>(); }
		}
	}
}

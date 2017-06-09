using FluentNHibernate.Cfg.Db;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Better postgre sql configuration<br/>
	/// 更好的PostgreSQL配置类<br/>
	/// </summary>
	public class BetterPostgreSQLConfiguration : PostgreSQLConfiguration {
		/// <summary>
		/// Better configuration<br/>
		/// 获取更好的配置<br/>
		/// </summary>
		public static PostgreSQLConfiguration Better {
			get { return new BetterPostgreSQLConfiguration().Dialect<BetterPostgreSQLDialect>(); }
		}
	}
}

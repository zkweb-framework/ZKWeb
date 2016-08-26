using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Sqlite confirguation for Microsoft.Data.Sqlite
	/// See: http://stackoverflow.com/questions/7626251/using-nhibernate-and-mono-data-sqlite
	/// </summary>
	internal class MicrosoftSQLiteConfiguration :
		PersistenceConfiguration<MicrosoftSQLiteConfiguration> {
		/// <summary>
		/// Default configuration
		/// </summary>
		public static MicrosoftSQLiteConfiguration Standard {
			get { return new MicrosoftSQLiteConfiguration(); }
		}

		/// <summary>
		/// Initialize
		/// </summary>
		public MicrosoftSQLiteConfiguration() {
			Driver<MicrosoftSQLiteDriver>();
			Dialect<SQLiteDialect>();
			Raw("query.substitutions", "true=1;false=0");
		}
	}
}

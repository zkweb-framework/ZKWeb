using NHibernate.Driver;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// Sqlite driver for Microsoft.Data.Sqlite
	/// See: http://stackoverflow.com/questions/7626251/using-nhibernate-and-mono-data-sqlite
	/// </summary>
	public class MicrosoftSQLiteDriver : ReflectionBasedDriver {
		/// <summary>
		/// Initialize
		/// </summary>
		public MicrosoftSQLiteDriver() : base(
			"Microsoft.Data.Sqlite",
			"Microsoft.Data.Sqlite",
			"Microsoft.Data.Sqlite.SqliteConnection",
			"Microsoft.Data.Sqlite.SqliteCommand") { }

#pragma warning disable CS1591
		public override bool UseNamedPrefixInParameter { get { return true; } }
		public override bool UseNamedPrefixInSql { get { return true; } }
		public override string NamedPrefix { get { return "@"; } }
		public override bool SupportsMultipleOpenReaders { get { return false; } }
#pragma warning restore CS1591
	}
}

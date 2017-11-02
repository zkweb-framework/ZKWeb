using System.Data;

namespace ZKWeb.Database.Wrappers {
	/// <summary>
	/// Wrapper for IDbConnection, use to log commands<br/>
	/// IDbConnection的包装类, 用于记录命令<br/>
	/// </summary>
	public class DbConnectionWrapper : IDbConnection {
#pragma warning disable CS1591
		private IDbConnection Connection;
		private IDatabaseContext Context;

		public DbConnectionWrapper(IDbConnection connection, IDatabaseContext context) {
			Connection = connection;
			Context = context;
		}

		public string ConnectionString {
			get => Connection.ConnectionString;
			set => Connection.ConnectionString = value;
		}

		public int ConnectionTimeout => Connection.ConnectionTimeout;

		public string Database => Connection.Database;

		public ConnectionState State => Connection.State;

		public IDbTransaction BeginTransaction() {
			return Connection.BeginTransaction();
		}

		public IDbTransaction BeginTransaction(IsolationLevel il) {
			return Connection.BeginTransaction(il);
		}

		public void ChangeDatabase(string databaseName) {
			Connection.ChangeDatabase(databaseName);
		}

		public void Close() {
			Connection.Close();
		}

		public IDbCommand CreateCommand() {
			var command = Connection.CreateCommand();
			return new DbCommandWrapper(command, Context);
		}

		public void Dispose() {
			Connection.Dispose();
		}

		public void Open() {
			Connection.Open();
		}
#pragma warning restore CS1591
	}
}

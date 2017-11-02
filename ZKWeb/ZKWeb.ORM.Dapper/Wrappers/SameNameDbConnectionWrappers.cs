using System.Data;
using ZKWeb.Database;
using ZKWeb.Database.Wrappers;

namespace ZKWeb.ORM.Dapper.Wrappers {
#pragma warning disable CS1591
	/// <summary>
	/// Dommel will determine database type by DbConnection type's name, so we need these types<br/>
	/// Dommel会根据DbConnection的类型名称判断数据库的类型, 所以我们需要这些类型<br/>
	/// </summary>
	internal class SqlConnection : DbConnectionWrapper {
		public SqlConnection(
			IDbConnection connection, IDatabaseContext context) : base(connection, context) {
		}
	}

	internal class SqliteConnection : DbConnectionWrapper {
		public SqliteConnection(
			IDbConnection connection, IDatabaseContext context) : base(connection, context) {
		}
	}

	internal class MySqlConnection : DbConnectionWrapper {
		public MySqlConnection(
			IDbConnection connection, IDatabaseContext context) : base(connection, context) {
		}
	}

	internal class NpgsqlConnection : DbConnectionWrapper {
		public NpgsqlConnection(
			IDbConnection connection, IDatabaseContext context) : base(connection, context) {
		}
	}
#pragma warning restore CS1591
}

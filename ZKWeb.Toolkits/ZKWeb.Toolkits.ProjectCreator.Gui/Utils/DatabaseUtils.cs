using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace ZKWeb.Toolkits.ProjectCreator.Gui.Utils {
	/// <summary>
	/// 数据库的工具函数
	/// </summary>
	public static class DatabaseUtils {
		/// <summary>
		/// 测试连接字符串是否可用
		/// </summary>
		/// <param name="database">数据库</param>
		/// <param name="connectionString">连接字符串</param>
		public static void TestConnectionString(string database, string connectionString) {
			if (database == "mssql") {
				using (var connection = new SqlConnection(connectionString)) {
					connection.Open();
				}
			} else if (database == "mysql") {
				using (var connection = new MySqlConnection(connectionString)) {
					connection.Open();
				}
			} else if (database == "postgresql") {
				using (var connection = new NpgsqlConnection(connectionString)) {
					connection.Open();
				}
			} else if (database == "sqlite") {
				var tempDir = Path.GetDirectoryName(Path.GetTempPath());
				connectionString = connectionString.Replace("{{App_Data}}", tempDir);
				using (var connection = new SQLiteConnection(connectionString)) {
					connection.Open();
				}
			} else {
				throw new ArgumentException($"Unsupported database {database}");
			}
		}
	}
}

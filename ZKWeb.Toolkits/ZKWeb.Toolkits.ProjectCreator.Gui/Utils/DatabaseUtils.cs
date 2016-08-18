using Microsoft.Data.Sqlite;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data.SqlClient;
using System.IO;

namespace ZKWeb.Toolkits.ProjectCreator.Gui.Utils {
	/// <summary>
	/// Database utility functions
	/// </summary>
	public static class DatabaseUtils {
		/// <summary>
		/// Test connection string is correct
		/// </summary>
		/// <param name="database">Database</param>
		/// <param name="connectionString">Connection string</param>
		public static void TestConnectionString(string database, string connectionString) {
			if (string.Compare(database, "MSSQL", true) == 0) {
				using (var connection = new SqlConnection(connectionString)) {
					connection.Open();
				}
			} else if (string.Compare(database, "MySQL", true) == 0) {
				using (var connection = new MySqlConnection(connectionString)) {
					connection.Open();
				}
			} else if (string.Compare(database, "PostgreSQL", true) == 0) {
				using (var connection = new NpgsqlConnection(connectionString)) {
					connection.Open();
				}
			} else if (string.Compare(database, "SQLite", true) == 0) {
				var tempDir = Path.GetDirectoryName(Path.GetTempPath());
				connectionString = connectionString.Replace("{{App_Data}}", tempDir);
				using (var connection = new SqliteConnection(connectionString)) {
					connection.Open();
				}
			} else if (string.Compare(database, "InMemory", true) == 0) {
				// Do nothing
			} else if (string.Compare(database, "MongoDB", true) == 0) {
				var mongoUrl = new MongoUrl(connectionString);
				var mongoDatabase = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
				mongoDatabase.ListCollections().ToBsonDocument();
			} else {
				throw new ArgumentException(
					$"Unsupported database {database}, please check it yourself");
			}
		}
	}
}

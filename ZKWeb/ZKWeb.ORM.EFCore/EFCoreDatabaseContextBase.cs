using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using ZKWeb.Storage;
using ZKWebStandard.Utils;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// A base database context only contains migration history table<br/>
	/// 一个数据库上下文的基类, 只包含迁移历史表<br/>
	/// </summary>
	public class EFCoreDatabaseContextBase : DbContext {
		/// <summary>
		/// Database type<br/>
		/// 数据库类型<br/>
		/// </summary>
		protected string DatabaseName { get; set; }
		/// <summary>
		/// Connection string<br/>
		/// 连接字符串<br/>
		/// </summary>
		protected string ConnectionString { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public EFCoreDatabaseContextBase(string database, string connectionString) {
			DatabaseName = database;
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Configure context options<br/>
		/// 配置上下文选项<br/>
		/// </summary>
		/// <param name="optionsBuilder">Options builder</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			if (string.Compare(DatabaseName, "MSSQL", true) == 0) {
				optionsBuilder.UseSqlServer(
					ConnectionString, option => option.UseRowNumberForPaging());
			} else if (string.Compare(DatabaseName, "SQLite", true) == 0) {
				optionsBuilder.UseSqlite(
					ConnectionString.Replace("{{App_Data}}", pathConfig.AppDataDirectory));
			} else if (string.Compare(DatabaseName, "MySQL", true) == 0) {
				optionsBuilder.UseMySql(ConnectionString);
			} else if (string.Compare(DatabaseName, "PostgreSQL", true) == 0) {
				optionsBuilder.UseNpgsql(ConnectionString);
			} else if (string.Compare(DatabaseName, "InMemory", true) == 0) {
				optionsBuilder.UseInMemoryDatabase(
					string.IsNullOrEmpty(ConnectionString) ?
					GuidUtils.SequentialGuid(DateTime.UtcNow).ToString() : ConnectionString);
			} else {
				throw new ArgumentException($"unsupported database type {Database}");
			}
			// EF 2.0 make some warnings as error, just ignore them
			optionsBuilder.ConfigureWarnings(w => w.Ignore(CoreEventId.IncludeIgnoredWarning));
		}

		/// <summary>
		/// Configure entity model<br/>
		/// 配置实体模型<br/>
		/// </summary>
		/// <param name="modelBuilder">Model builder</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			new EFCoreMigrationHistory().Configure(modelBuilder);
		}
	}
}

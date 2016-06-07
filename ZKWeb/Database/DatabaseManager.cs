using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ZKWeb.Database.Interfaces;
using ZKWeb.Server;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Database {
	/// <summary>
	/// 数据库管理器
	/// </summary>
	public class DatabaseManager {
		/// <summary>
		/// 数据库会话生成器
		/// </summary>
		public virtual ISessionFactory SessionFactory { get; protected set; }

		/// <summary>
		/// 获取数据库上下文
		/// 事务的隔离等级默认是IsolationLevel.ReadCommitted
		/// </summary>
		/// <returns></returns>
		public virtual DatabaseContext GetContext() {
			return GetContext(IsolationLevel.ReadCommitted);
		}

		/// <summary>
		/// 获取数据库上下文
		/// </summary>
		/// <param name="isolationLevel">事务的隔离等级</param>
		/// <returns></returns>
		public virtual DatabaseContext GetContext(IsolationLevel isolationLevel) {
			return new DatabaseContext(SessionFactory, isolationLevel);
		}

		/// <summary>
		/// 根据数据库类型和连接字符串创建数据库会话生成器
		/// 支持自动更新数据库
		/// </summary>
		/// <param name="database">数据库类型</param>
		/// <param name="connectionString">连接字符串</param>
		/// <param name="databaseScriptPath">判断是否需要更新数据库的文件路径，传入null时总是更新</param>
		/// <returns></returns>
		internal static ISessionFactory BuildSessionFactory(
			string database, string connectionString, string databaseScriptPath) {
			// 创建数据库配置
			IPersistenceConfigurer db;
			if (string.Compare(database, DatabaseTypes.PostgreSQL, true) == 0) {
				db = PostgreSQLConfiguration.Standard.ConnectionString(connectionString);
			} else if (string.Compare(database, DatabaseTypes.SQLite, true) == 0) {
				db = SQLiteConfiguration.Standard.ConnectionString(
					connectionString.Replace("{{App_Data}}", PathConfig.AppDataDirectory));
			} else if (string.Compare(database, DatabaseTypes.MySQL, true) == 0) {
				db = MySQLConfiguration.Standard.ConnectionString(connectionString);
			} else if (string.Compare(database, DatabaseTypes.MSSQL, true) == 0) {
				db = MsSqlConfiguration.MsSql2008.ConnectionString(connectionString);
			} else {
				throw new ArgumentException($"unknown database type {database}");
			}
			var configuration = Fluently.Configure();
			configuration.Database(db);
			// 注册数据表类型
			var tableTypes = Application.Ioc.ResolveMany<IMappingProvider>()
				.Select(p => p.GetType()).OrderBy(t => t.FullName).ToList();
			configuration.Mappings(m => tableTypes.ForEach(t => m.FluentMappings.Add(t)));
			// 调用数据库初始化处理器
			// 可以在这里全局处理表名和字段名
			var handlers = Application.Ioc.ResolveMany<IDatabaseInitializeHandler>();
			handlers.ForEach(h => h.OnInitialize(configuration));
			// 自动更新数据库
			// 流程 (databaseScriptPath不等于空时）
			// - 生成当前数据库表的sql脚本，不会实际执行
			// - 判断是否和databaseScriptPath对应的文件内容一致
			// - 不一致时执行数据库更新并保存到该文件中
			// 检测是否需要自动更新的原因是为了优化启动网站时的性能
			// 保存文件的处理要放到BuildSessionFactory后面，
			// 否则会出现数据库没有初始化成功但仍然保存了该文件的问题。
			Action onBuildFactorySuccess = null;
			configuration.ExposeConfiguration(c => {
				if (string.IsNullOrEmpty(databaseScriptPath)) {
					new SchemaUpdate(c).Execute(false, true);
				} else {
					var scriptBuilder = new StringBuilder();
					scriptBuilder.AppendLine("/* file for database migration checking, don't execute */");
					scriptBuilder.AppendLine(string.Format("/* {0} {1} */", database, connectionString));
					new SchemaExport(c).Create(s => scriptBuilder.AppendLine(s), false);
					var path = databaseScriptPath;
					var script = scriptBuilder.ToString();
					if (!File.Exists(path) || script != File.ReadAllText(path)) {
						new SchemaUpdate(c).Execute(false, true);
						onBuildFactorySuccess = () => File.WriteAllText(path, script);
					}
				}
			});
			// 应用更新并创建数据库会话生成器
			var sessionFactory = configuration.BuildSessionFactory();
			onBuildFactorySuccess?.Invoke();
			return sessionFactory;
		}
		
		/// <summary>
		/// 初始化数据库管理器
		/// </summary>
		internal static void Initialize() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var config = configManager.WebsiteConfig;
			var databaseManager = Application.Ioc.Resolve<DatabaseManager>();
			databaseManager.SessionFactory = BuildSessionFactory(
				config.Database, config.ConnectionString, PathConfig.DatabaseScriptPath);
		}
	}
}

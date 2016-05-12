using DryIoc;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
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
	/// 这个管理器创建前需要先创建以下管理器
	///		插件管理器
	/// </summary>
	public class DatabaseManager {
		/// <summary>
		/// 数据库会话生成器
		/// </summary>
		public ISessionFactory SessionFactory { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public DatabaseManager() {
			// 获取网站配置，判断使用的数据库配置
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var config = configManager.WebsiteConfig;
			IPersistenceConfigurer db;
			if (string.Compare(config.Database, DatabaseTypes.PostgreSQL, true) == 0) {
				db = PostgreSQLConfiguration.Standard.ConnectionString(config.ConnectionString);
			} else if (string.Compare(config.Database, DatabaseTypes.SQLite, true) == 0) {
				db = SQLiteConfiguration.Standard.ConnectionString(
					config.ConnectionString.Replace("{{App_Data}}", PathConfig.AppDataDirectory));
			} else if (string.Compare(config.Database, DatabaseTypes.MySQL, true) == 0) {
				db = MySQLConfiguration.Standard.ConnectionString(config.ConnectionString);
			} else if (string.Compare(config.Database, DatabaseTypes.MSSQL, true) == 0) {
				db = MsSqlConfiguration.MsSql2008.ConnectionString(config.ConnectionString);
			} else {
				throw new ArgumentException($"unknow database type {config.Database}");
			}
			// 初始化数据库
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
			// 流程
			// - 生成当前数据库表的sql脚本，不会实际执行
			// - 判断是否和App_Data/DatabaseScript.txt一致
			// - 不一致时执行数据库更新并保存到该文件中
			// 检测是否需要自动更新的原因是为了优化启动网站时的性能
			// 保存文件的处理要放到BuildSessionFactory后面，
			// 否则会出现数据库没有初始化成功但仍然保存了该文件的问题。
			Action onFactorySuccess = () => { };
			configuration.ExposeConfiguration(c => {
				var scriptBuilder = new StringBuilder(
					"/* this file is generated for checking database should update or not, don't execute */\r\n");
				scriptBuilder.AppendFormat("/* {0} {1} */\r\n", config.Database, config.ConnectionString);
				var path = PathConfig.DatabaseScriptPath;
				new SchemaExport(c).Create(s => scriptBuilder.Append(s).Append("\r\n"), false);
				var script = scriptBuilder.ToString();
				if (!File.Exists(path) || script != File.ReadAllText(path)) {
					new SchemaUpdate(c).Execute(false, true);
					onFactorySuccess = () => File.WriteAllText(path, script);
				}
			});
			// 应用更新并创建数据库会话生成器
			SessionFactory = configuration.BuildSessionFactory();
			onFactorySuccess();
		}

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
	}
}

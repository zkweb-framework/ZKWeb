using DryIoc;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Core {
	/// <summary>
	/// 数据库管理器
	/// 这个管理器创建前需要先创建以下管理器
	///		插件管理器
	/// </summary>
	public class DatabaseManager {
		/// <summary>
		/// 数据库会话生成器
		/// </summary>
		public ISessionFactory SessionFactory { get; set; }

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
				db = SQLiteConfiguration.Standard.ConnectionString(config.ConnectionString);
			} else if (string.Compare(config.Database, DatabaseTypes.MySQL, true) == 0) {
				db = MySQLConfiguration.Standard.ConnectionString(config.ConnectionString);
			} else if (string.Compare(config.Database, DatabaseTypes.MSSQL, true) == 0) {
				db = MsSqlConfiguration.MsSql2008.ConnectionString(config.ConnectionString);
			} else {
				throw new ArgumentException($"unknow database type {config.Database}");
			}
			// 获取插件中注册的所有数据表类型
			var tableTypes = Application.Ioc.ResolveMany<IMappingProvider>()
				.Select(p => p.GetType()).OrderBy(t => t.FullName).ToList();
			// 创建数据库会话生成器
			SessionFactory = Fluently.Configure()
				.Database(db)
				.Mappings(m => tableTypes.ForEach(t => m.FluentMappings.Add(t)))
				.ExposeConfiguration(c => {
					// 自动更新数据库流程
					//	生成当前数据库表的sql脚本，不会实际执行
					//	判断是否和App_Data/DatabaseScript.txt一致
					//	不一致时执行数据库更新并保存到该文件中
					// 检测是否需要自动更新的原因是为了优化启动网站时的性能
					var scriptBuilder = new StringBuilder(
						"/* this file is generated for checking database should update or not, don't execute */\r\n");
					var path = PathConfig.DatabaseScriptPath;
					new SchemaExport(c).Create(s => scriptBuilder.Append(s).Append("\r\n"), false);
					var script = scriptBuilder.ToString();
					if (!File.Exists(path) || script != File.ReadAllText(path)) {
						new SchemaUpdate(c).Execute(false, true);
						File.WriteAllText(path, script); // 成功后再写入
					}
				})
				.BuildSessionFactory();
		}

		/// <summary>
		/// 获取数据库上下文
		/// </summary>
		/// <returns></returns>
		public DatabaseContext GetContext() {
			return new DatabaseContext(SessionFactory);
		}
	}
}

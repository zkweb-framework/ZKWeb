using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using System.Linq;
using ZKWeb.Database;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using System;
using System.Text;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using ZKWeb.Logging;
using ZKWeb.Storage;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// NHibernate database context factory<br/>
	/// NHibernate的数据库上下文生成器<br/>
	/// </summary>
	public class NHibernateDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Batch size<br/>
		/// 批量操作的命令数量<br/>
		/// </summary>
		protected const int BatchSize = 1024;
		/// <summary>
		/// Database type<br/>
		/// 数据库类型<br/>
		/// </summary>
		protected string Database { get; set; }
		/// <summary>
		/// NHibernate session factory<br/>
		/// NHibernate的会话生成器<br/>
		/// </summary>
		protected ISessionFactory SessionFactory { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public NHibernateDatabaseContextFactory(string database, string connectionString) {
			// create database configuration
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			var fileStorage = Application.Ioc.Resolve<IFileStorage>();
			IPersistenceConfigurer db;
			if (string.Compare(database, "PostgreSQL", true) == 0) {
				db = BetterPostgreSQLConfiguration.Better.ConnectionString(connectionString);
			} else if (string.Compare(database, "SQLite", true) == 0) {
				db = SQLiteConfiguration.Standard.ConnectionString(
					connectionString.Replace("{{App_Data}}", pathConfig.AppDataDirectory));
			} else if (string.Compare(database, "MySQL", true) == 0) {
				db = MySQLConfiguration.Standard.ConnectionString(connectionString);
			} else if (string.Compare(database, "MSSQL", true) == 0) {
				db = MsSqlConfiguration.MsSql2008.ConnectionString(connectionString);
			} else {
				throw new ArgumentException($"unsupported database type {database}");
			}
			var configuration = Fluently.Configure();
			configuration.Database(db);
			// register entity mappings
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider>();
			var entityTypes = providers
				.Select(p => ReflectionUtils.GetGenericArguments(
					p.GetType(), typeof(IEntityMappingProvider<>))[0])
				.Distinct().ToList();
			configuration.Mappings(m => {
				foreach (var entityType in entityTypes) {
					var builder = typeof(NHibernateEntityMappingBuilder<>).MakeGenericType(entityType);
					m.FluentMappings.Add(builder);
				}
			});
			// set many-to-many table name
			var handlers = Application.Ioc.ResolveMany<IDatabaseInitializeHandler>();
			configuration.Mappings(m => {
				m.FluentMappings.Conventions.Add(ConventionBuilder.HasManyToMany.Always(x => {
					var tableName = string.Format("{0}To{1}", x.EntityType.Name, x.ChildType.Name);
					foreach (var handler in handlers) {
						handler.ConvertTableName(ref tableName);
					}
					x.Table(tableName);
				}));
			});
			// initialize database scheme
			// flow:
			// - generate ddl script
			// - compare to App_Data\nh_{hash}.ddl
			// - if they are different, upgrade database scheme and write ddl script to file
			// it can make the website startup faster
			var hash = PasswordUtils.Sha1Sum(
				Encoding.UTF8.GetBytes(database + connectionString)).ToHex();
			var ddlFileEntry = fileStorage.GetStorageFile($"nh_{hash}.ddl");
			Action onBuildFactorySuccess = null;
			configuration.ExposeConfiguration(c => {
				var scriptBuilder = new StringBuilder();
				scriptBuilder.AppendLine("/* this file is for database migration checking, don't execute it */");
				new SchemaExport(c).Create(s => scriptBuilder.AppendLine(s), false);
				var script = scriptBuilder.ToString();
				if (!ddlFileEntry.Exists || script != ddlFileEntry.ReadAllText()) {
					var logManager = Application.Ioc.Resolve<LogManager>();
					var schemaUpdate = new SchemaUpdate(c);
					schemaUpdate.Execute(false, true);
					foreach (var ex in schemaUpdate.Exceptions) {
						logManager.LogError($"NHibernate schema update error: ({ex.GetType()}) {ex.Message}");
					}
					onBuildFactorySuccess = () => ddlFileEntry.WriteAllText(script);
				}
			});
			// create nhibernate session factory and write ddl script to file
			Database = database;
			SessionFactory = configuration.BuildSessionFactory();
			onBuildFactorySuccess?.Invoke();
		}

		/// <summary>
		/// Create database context<br/>
		/// 创建数据库上下文<br/>
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			var session = SessionFactory.OpenSession();
			try { session.SetBatchSize(BatchSize); } catch (NotSupportedException) { }
			return new NHibernateDatabaseContext(session, Database);
		}
	}
}

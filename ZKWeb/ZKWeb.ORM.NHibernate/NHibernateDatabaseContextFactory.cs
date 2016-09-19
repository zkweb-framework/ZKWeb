using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using System.Linq;
using ZKWeb.Database;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using System;
using System.IO;
using System.Text;
using NHibernate.Tool.hbm2ddl;
using NHibernate;

namespace ZKWeb.ORM.NHibernate {
	/// <summary>
	/// NHibernate database context factory
	/// </summary>
	internal class NHibernateDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Database type
		/// </summary>
		private string Database { get; set; }
		/// <summary>
		/// NHibernate session factory
		/// </summary>
		private ISessionFactory SessionFactory { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public NHibernateDatabaseContextFactory(string database, string connectionString) {
			// create database configuration
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
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
			var entityTypes = providers.Select(p =>
				ReflectionUtils.GetGenericArguments(
				p.GetType(), typeof(IEntityMappingProvider<>))[0]).ToList();
			configuration.Mappings(m => entityTypes.ForEach(t => m.FluentMappings.Add(
				typeof(NHibernateEntityMappingBuilder<>).MakeGenericType(t))));
			// call initialize handlers
			var handlers = Application.Ioc.ResolveMany<IDatabaseInitializeHandler>();
			configuration.Mappings(m => {
				m.FluentMappings.Conventions.Add(ConventionBuilder.Class.Always(x => {
					var tableName = x.EntityType.Name;
					handlers.ForEach(h => h.ConvertTableName(ref tableName));
					x.Table(tableName);
				}));
				m.FluentMappings.Conventions.Add(ConventionBuilder.HasManyToMany.Always(x => {
					var tableName = string.Format(
						"{0}To{1}", x.EntityType.Name, x.ChildType.Name);
					handlers.ForEach(h => h.ConvertTableName(ref tableName));
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
			var ddlPath = Path.Combine(
				pathConfig.AppDataDirectory, string.Format("nh_{0}.ddl", hash));
			Action onBuildFactorySuccess = null;
			configuration.ExposeConfiguration(c => {
				var scriptBuilder = new StringBuilder();
				scriptBuilder.AppendLine("/* this file is for database migration checking, don't execute it */");
				new SchemaExport(c).Create(s => scriptBuilder.AppendLine(s), false);
				var script = scriptBuilder.ToString();
				if (!File.Exists(ddlPath) || script != File.ReadAllText(ddlPath)) {
					var schemaUpdate = new SchemaUpdate(c);
					schemaUpdate.Execute(false, true);
					if (schemaUpdate.Exceptions.Any()) {
						throw schemaUpdate.Exceptions.First();
					}
					onBuildFactorySuccess = () => File.WriteAllText(ddlPath, script);
				}
			});
			// create nhibernate session factory and write ddl script to file
			Database = database;
			SessionFactory = configuration.BuildSessionFactory();
			onBuildFactorySuccess?.Invoke();
		}

		/// <summary>
		/// Create database context
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			var session = SessionFactory.OpenSession();
			return new NHibernateDatabaseContext(session, Database);
		}
	}
}

using System;
using System.Linq;
using System.Reflection;
using ZKWeb.Plugin.AssemblyLoaders;
using ZKWeb.Server;

namespace ZKWeb.Database {
	/// <summary>
	/// Database manager
	/// </summary>
	public class DatabaseManager {
		/// <summary>
		/// Default database context factory
		/// </summary>
		protected virtual IDatabaseContextFactory DefaultContextFactory { get; set; }

		/// <summary>
		/// Create database context from the default factory
		/// </summary>
		/// <returns></returns>
		public virtual IDatabaseContext CreateContext() {
			return DefaultContextFactory.CreateContext();
		}

		/// <summary>
		/// Create database context factory from the given parameters
		/// </summary>
		/// <param name="orm">Object relational mapper</param>
		/// <param name="database">Database name</param>
		/// <param name="connectionString">Database connection string</param>
		/// <returns></returns>
		public static IDatabaseContextFactory CreateContextFactor(
			string orm, string database, string connectionString) {
			if (string.IsNullOrEmpty(orm)) {
				throw new NotSupportedException("No ORM name is provided, please set it first");
			}
			var assemblyName = string.Format("ZKWeb.ORM.{0}", orm);
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			Assembly assembly;
			try {
				assembly = assemblyLoader.Load(assemblyName);
			} catch (Exception e) {
				throw new NotSupportedException(string.Format(
					"Load ORM assembly {0} failed, please install it first. error: {1}", orm, e.Message));
			}
			var factorType = assembly.GetTypes().FirstOrDefault(t =>
				typeof(IDatabaseContextFactory).IsAssignableFrom(t));
			if (factorType == null) {
				throw new NotSupportedException(string.Format(
					"Find factory type from ORM {0} failed", orm));
			}
			return (IDatabaseContextFactory)Activator.CreateInstance(factorType, database, connectionString);
		}

		/// <summary>
		/// Initialize database manager
		/// </summary>
		internal static void Initialize() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var config = configManager.WebsiteConfig;
			var databaseManager = Application.Ioc.Resolve<DatabaseManager>();
			databaseManager.DefaultContextFactory = CreateContextFactor(
				config.ORM, config.Database, config.ConnectionString);
		}
	}
}

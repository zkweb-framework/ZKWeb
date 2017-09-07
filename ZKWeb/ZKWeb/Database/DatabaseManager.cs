using System;
using System.Linq;
using System.Reflection;
using ZKWeb.Plugin.AssemblyLoaders;
using ZKWeb.Server;

namespace ZKWeb.Database {
	/// <summary>
	/// Database manager<br/>
	/// 数据库管理器<br/>
	/// </summary>
	/// <example>
	/// <code>
	/// var databaseManager = Application.Ioc.Resolve&lt;DatabaseManager&gt;();
	///	using (var context = databaseManager.CreateContext()) {
	///		var data = new ExampleTable() {
	///			Name = "test",
	///			CreateTime = DateTime.UtcNow,
	///			Deleted = false
	///		};
	///		context.Save(ref data);
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="IDatabaseContext"/>
	/// <seealso cref="IDatabaseContextFactory"/>
	public class DatabaseManager {
		/// <summary>
		/// Default database context factory<br/>
		/// 默认的数据库上下文生成器<br/>
		/// </summary>
		protected internal virtual IDatabaseContextFactory DefaultContextFactory { get; set; }

		/// <summary>
		/// Create database context from the default factory<br/>
		/// 使用默认的生成器创建数据库上下文<br/>
		/// </summary>
		/// <returns></returns>
		public virtual IDatabaseContext CreateContext() {
			return DefaultContextFactory.CreateContext();
		}

		/// <summary>
		/// Create database context factory from the given parameters<br/>
		/// 根据传入的参数创建数据库上下文生成器<br/>
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
		/// Initialize database manager<br/>
		/// 初始化数据库管理器<br/>
		/// </summary>
		internal protected virtual void Initialize() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var config = configManager.WebsiteConfig;
			DefaultContextFactory = CreateContextFactor(
				config.ORM, config.Database, config.ConnectionString);
		}
	}
}

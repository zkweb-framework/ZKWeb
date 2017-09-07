using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZKWeb.Database;
using ZKWeb.Plugin;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Testing.Events;

namespace ZKWeb.Testing {
	/// <summary>
	/// Testing manager<br/>
	/// 测试管理器<br/>
	/// </summary>
	public class TestManager {
		/// <summary>
		/// Get assemblies for testing<br/>
		/// 获取测试使用的所有程序集<br/>
		/// </summary>
		/// <returns></returns>
		public virtual IList<Assembly> GetAssembliesForTest() {
			var result = new List<Assembly>();
			result.Add(typeof(TestManager).Assembly); // ZKWeb
			result.Add(typeof(TestRunner).Assembly); // ZKWebStandard
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			result.AddRange(pluginManager.PluginAssemblies); // 插件程序集列表}
			return result;
		}

		/// <summary>
		/// Run tests from assembly<br/>
		/// Will block until all tests finished<br/>
		/// 运行指定程序集中的所有测试<br/>
		/// 会阻塞到运行完成<br/>
		/// </summary>
		/// <param name="assembly">Assembly</param>
		/// <param name="eventHandler">Extra test event handler, can be null</param>
		public virtual void RunAssemblyTest(
			Assembly assembly, ITestEventHandler eventHandler = null) {
			// Get registered test event handlers
			var eventHandlers = Application.Ioc.ResolveMany<ITestEventHandler>().ToList();
			if (eventHandler != null) {
				eventHandlers.Add(eventHandler);
			}
			// Create runner and execute tests
			var runner = new TestRunner(assembly, eventHandlers);
			runner.Run();
		}

		/// <summary>
		/// Run tests from all assemblies<br/>
		/// Will block until all tests finished<br/>
		/// 运行所有程序集中的所有测试<br/>
		/// 会阻塞到运行完成<br/>
		/// </summary>
		/// <param name="eventHandler">Extra test event handler, can be null</param>
		public virtual void RunAllAssemblyTest(ITestEventHandler eventHandler = null) {
			foreach (var assembly in GetAssembliesForTest()) {
				RunAssemblyTest(assembly, eventHandler);
			}
		}

		/// <summary>
		/// Use temporary database in the specified scope<br/>
		/// 在指定范围内使用临时数据库<br/>
		/// </summary>
		/// <returns></returns>
		public virtual IDisposable UseTemporaryDatabase() {
			// Create database context factory, default use inmemory orm
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var extra = configManager.WebsiteConfig.Extra;
			var orm = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseORM) ?? "InMemory";
			var database = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseType);
			var connectionString = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseConnectionString);
			var contextFactory = DatabaseManager.CreateContextFactor(orm, database, connectionString);
			// Override database manager with above factory
			var overrideIoc = Application.OverrideIoc();
			var databaseManagerMock = new DatabaseManager();
			databaseManagerMock.DefaultContextFactory = contextFactory;
			Application.Ioc.Unregister<DatabaseManager>();
			Application.Ioc.RegisterInstance(databaseManagerMock);
			// Finish override when disposed
			return overrideIoc;
		}
	}
}

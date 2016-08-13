using NSubstitute;
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
	/// Testing manager
	/// </summary>
	public class TestManager {
		/// <summary>
		/// Get assemblies for testing
		/// </summary>
		/// <returns></returns>
		public virtual IList<Assembly> GetAssembliesForTest() {
			var result = new List<Assembly>();
			result.Add(typeof(TestManager).GetTypeInfo().Assembly); // ZKWeb
			result.Add(typeof(TestRunner).GetTypeInfo().Assembly); // ZKWebStandard
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			result.AddRange(pluginManager.PluginAssemblies); // 插件程序集列表}
			return result;
		}

		/// <summary>
		/// Run tests from assembly
		/// Will block until all tests finished
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
		/// Run tests from assembly
		/// Will block until all tests finished
		/// </summary>
		/// <param name="eventHandler">Extra test event handler, can be null</param>
		public virtual void RunAllAssemblyTest(ITestEventHandler eventHandler = null) {
			foreach (var assembly in GetAssembliesForTest()) {
				RunAssemblyTest(assembly, eventHandler);
			}
		}

		/// <summary>
		/// Use temporary database in the specified scope
		/// </summary>
		/// <returns></returns>
		public virtual IDisposable UseTemporaryDatabase() {
			// Create database context factory, default use inmemory orm
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var extra = configManager.WebsiteConfig.Extra;
			var orm = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseORM) ?? "InMemory";
			var database = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseType);
			var connectionString = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseConnectionString);
			var contextFactory = DatabaseManager.CreateContextFactor(orm, database, connectionString);
			// Override database manager with above factory
			var overrideIoc = Application.OverrideIoc();
			var databaseManagerMock = Substitute.For<DatabaseManager>();
			databaseManagerMock.CreateContext().Returns(callInfo => contextFactory.CreateContext());
			Application.Ioc.Unregister<DatabaseManager>();
			Application.Ioc.RegisterInstance(databaseManagerMock);
			// Finish override when disposed
			return overrideIoc;
		}
	}
}

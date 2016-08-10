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
	/// 测试管理器
	/// </summary>
	public class TestManager {
		/// <summary>
		/// 获取测试使用的程序集列表
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
		/// 运行指定程序集的测试
		/// 会阻塞到运行结束
		/// </summary>
		/// <param name="assembly">程序集</param>
		/// <param name="eventHandler">附加的事件处理器，可以等于null</param>
		public virtual void RunAssemblyTest(
			Assembly assembly, ITestEventHandler eventHandler = null) {
			// 获取注册的事件处理器列表
			var eventHandlers = Application.Ioc.ResolveMany<ITestEventHandler>().ToList();
			if (eventHandler != null) {
				eventHandlers.Add(eventHandler);
			}
			// 创建运行器并执行测试
			var runner = new TestRunner(assembly, eventHandlers);
			runner.Run();
		}

		/// <summary>
		/// 运行所有程序集的测试
		/// 会阻塞到运行结束
		/// </summary>
		/// <param name="eventHandler">附加的事件处理器，可以等于null</param>
		public virtual void RunAllAssemblyTest(ITestEventHandler eventHandler = null) {
			foreach (var assembly in GetAssembliesForTest()) {
				RunAssemblyTest(assembly, eventHandler);
			}
		}
		
		/// <summary>
		/// 在指定的范围内启用临时数据库
		/// </summary>
		/// <returns></returns>
		public virtual IDisposable UseTemporaryDatabase() {
			// 创建数据库会话生成器
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var extra = configManager.WebsiteConfig.Extra;
			var orm = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseORM) ?? "InMemory";
			var database = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseType);
			var connectionString = extra.GetOrDefault<string>(ExtraConfigKeys.TemporaryDatabaseConnectionString);
			var contextFactory = DatabaseManager.CreateContextFactor(orm, database, connectionString);
			// 重载当前的数据库管理器，使用刚才创建的数据库会话生成器
			var overrideIoc = Application.OverrideIoc();
			var databaseManagerMock = Substitute.For<DatabaseManager>();
			databaseManagerMock.CreateContext().Returns(callInfo => contextFactory.CreateContext());
			Application.Ioc.Unregister<DatabaseManager>();
			Application.Ioc.RegisterInstance(databaseManagerMock);
			// 区域结束后结束对容器的重载
			return overrideIoc;
		}
	}
}

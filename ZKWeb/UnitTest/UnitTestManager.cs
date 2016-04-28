using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using ZKWeb.Plugin;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Utils.UnitTest.Event;

namespace ZKWeb.UnitTest {
	/// <summary>
	/// 单元测试管理器
	/// </summary>
	public class UnitTestManager {
		/// <summary>
		/// 获取单元测试使用的程序集列表
		/// </summary>
		/// <returns></returns>
		public virtual IList<Assembly> GetAssembliesForTest() {
			var result = new List<Assembly>();
			result.Add(typeof(UnitTestManager).Assembly); // ZKWeb
			result.Add(typeof(UnitTestRunner).Assembly); // ZKWeb.Utils
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
			Assembly assembly, IUnitTestEventHandler eventHandler = null) {
			// 获取注册的事件处理器列表
			var eventHandlers = Application.Ioc.ResolveMany<IUnitTestEventHandler>().ToList();
			if (eventHandler != null) {
				eventHandlers.Add(eventHandler);
			}
			// 创建运行器并执行测试
			var runner = new UnitTestRunner(assembly, eventHandlers);
			runner.Run();
		}

		/// <summary>
		/// 运行所有程序集的测试
		/// 会阻塞到运行结束
		/// </summary>
		/// <param name="eventHandler">附加的事件处理器，可以等于null</param>
		public virtual void RunAllAssemblyTest(IUnitTestEventHandler eventHandler = null) {
			foreach (var assembly in GetAssembliesForTest()) {
				RunAssemblyTest(assembly, eventHandler);
			}
		}
	}
}

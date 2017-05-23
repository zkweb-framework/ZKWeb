using System;
using System.Reflection;

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for single test failed<br/>
	/// 单个测试失败的信息<br/>
	/// </summary>
	public class TestFailedInfo {
		/// <summary>
		/// Test runner<br/>
		/// 测试运行器<br/>
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Test method<br/>
		/// 测试函数<br/>
		/// </summary>
		public MethodInfo Method { get; private set; }
		/// <summary>
		/// Test instance<br/>
		/// 测试实例<br/>
		/// </summary>
		public object Instance { get; private set; }
		/// <summary>
		/// Test failed exception<br/>
		/// 测试失败的例外<br/>
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="method">Test method</param>
		/// <param name="instance">Test instance</param>
		/// <param name="exception">Test failed exception</param>
		public TestFailedInfo(
			TestRunner runner, MethodInfo method, object instance, Exception exception) {
			Runner = runner;
			Method = method;
			Instance = instance;
			Exception = exception;
		}
	}
}

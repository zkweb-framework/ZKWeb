using System.Reflection;

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for single test starting<br/>
	/// 单个测试开始的信息<br/>
	/// </summary>
	public class TestStartingInfo {
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
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="method">Test method</param>
		/// <param name="instance">Test instance</param>
		public TestStartingInfo(TestRunner runner, MethodInfo method, object instance) {
			Runner = runner;
			Method = method;
			Instance = instance;
		}
	}
}

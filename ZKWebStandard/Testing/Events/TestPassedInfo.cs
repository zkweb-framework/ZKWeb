using System.Reflection;

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// 单项测试通过时的信息
	/// </summary>
	public class TestPassedInfo {
		/// <summary>
		/// 测试运行器
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// 测试函数
		/// </summary>
		public MethodInfo Method { get; private set; }
		/// <summary>
		/// 测试类的实例
		/// </summary>
		public object Instance { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="runner">测试运行器</param>
		/// <param name="method">测试函数</param>
		/// <param name="instance">测试类的实例</param>
		public TestPassedInfo(
			TestRunner runner, MethodInfo method, object instance) {
			Runner = runner;
			Method = method;
			Instance = instance;
		}
	}
}

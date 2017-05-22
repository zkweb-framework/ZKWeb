using System.Reflection;

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for single test passed<br/>
	/// <br/>
	/// </summary>
	public class TestPassedInfo {
		/// <summary>
		/// Test runner<br/>
		/// <br/>
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Test method<br/>
		/// <br/>
		/// </summary>
		public MethodInfo Method { get; private set; }
		/// <summary>
		/// Test instance<br/>
		/// <br/>
		/// </summary>
		public object Instance { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="method">Test method</param>
		/// <param name="instance">Test instance</param>
		public TestPassedInfo(
			TestRunner runner, MethodInfo method, object instance) {
			Runner = runner;
			Method = method;
			Instance = instance;
		}
	}
}

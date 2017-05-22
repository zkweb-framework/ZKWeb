using System.Reflection;

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for single test skipped<br/>
	/// <br/>
	/// </summary>
	public class TestSkippedInfo {
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
		/// Test skipped exception<br/>
		/// <br/>
		/// </summary>
		public AssertSkipedException Exception { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="method">Test method</param>
		/// <param name="instance">Test instance</param>
		/// <param name="exception">Test skipped exception</param>
		public TestSkippedInfo(
			TestRunner runner, MethodInfo method, object instance, AssertSkipedException exception) {
			Runner = runner;
			Method = method;
			Instance = instance;
			Exception = exception;
		}
	}
}

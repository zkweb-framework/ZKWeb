using System.Reflection;

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for single test skipped
	/// </summary>
	public class TestSkippedInfo {
		/// <summary>
		/// Test runner
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Test method
		/// </summary>
		public MethodInfo Method { get; private set; }
		/// <summary>
		/// Test instance
		/// </summary>
		public object Instance { get; private set; }
		/// <summary>
		/// Test skipped exception
		/// </summary>
		public AssertSkipedException Exception { get; private set; }

		/// <summary>
		/// Initialize
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

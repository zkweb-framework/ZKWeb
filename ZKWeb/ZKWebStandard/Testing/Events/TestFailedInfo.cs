using System;
using System.Reflection;

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for single test failed
	/// </summary>
	public class TestFailedInfo {
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
		/// Test failed exception
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// Initialize
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

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for debug message
	/// </summary>
	public class DebugMessageInfo {
		/// <summary>
		/// Test runner
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Debug message
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="message">Debug message</param>
		public DebugMessageInfo(TestRunner runner, string message) {
			Runner = runner;
			Message = message;
		}
	}
}

namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for debug message<br/>
	/// <br/>
	/// </summary>
	public class DebugMessageInfo {
		/// <summary>
		/// Test runner<br/>
		/// <br/>
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Debug message<br/>
		/// <br/>
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="message">Debug message</param>
		public DebugMessageInfo(TestRunner runner, string message) {
			Runner = runner;
			Message = message;
		}
	}
}

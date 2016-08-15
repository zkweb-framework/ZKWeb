namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for error message
	/// </summary>
	public class ErrorMessageInfo {
		/// <summary>
		/// Test runner
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Error message
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="message">Error message</param>
		public ErrorMessageInfo(TestRunner runner, string message) {
			Runner = runner;
			Message = message;
		}
	}
}

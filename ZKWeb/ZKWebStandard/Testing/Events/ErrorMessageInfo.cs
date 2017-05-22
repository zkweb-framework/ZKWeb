namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for error message<br/>
	/// <br/>
	/// </summary>
	public class ErrorMessageInfo {
		/// <summary>
		/// Test runner<br/>
		/// <br/>
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Error message<br/>
		/// <br/>
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="message">Error message</param>
		public ErrorMessageInfo(TestRunner runner, string message) {
			Runner = runner;
			Message = message;
		}
	}
}

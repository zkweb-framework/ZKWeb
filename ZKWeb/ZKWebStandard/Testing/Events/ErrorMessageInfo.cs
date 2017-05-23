namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for error message<br/>
	/// 错误消息的信息<br/>
	/// </summary>
	public class ErrorMessageInfo {
		/// <summary>
		/// Test runner<br/>
		/// 测试运行器<br/>
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Error message<br/>
		/// 错误消息<br/>
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="message">Error message</param>
		public ErrorMessageInfo(TestRunner runner, string message) {
			Runner = runner;
			Message = message;
		}
	}
}

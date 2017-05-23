namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for debug message<br/>
	/// 除错消息的信息<br/>
	/// </summary>
	public class DebugMessageInfo {
		/// <summary>
		/// Test runner<br/>
		/// 测试运行器<br/>
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Debug message<br/>
		/// 除错消息<br/>
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="message">Debug message</param>
		public DebugMessageInfo(TestRunner runner, string message) {
			Runner = runner;
			Message = message;
		}
	}
}

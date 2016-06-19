namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// 额外的错误信息
	/// </summary>
	public class ErrorMessageInfo {
		/// <summary>
		/// 测试运行器
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// 错误信息
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="runner">测试运行器</param>
		/// <param name="message">错误信息</param>
		public ErrorMessageInfo(TestRunner runner, string message) {
			Runner = runner;
			Message = message;
		}
	}
}

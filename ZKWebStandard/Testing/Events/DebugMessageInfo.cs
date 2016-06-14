namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// 额外的除错信息
	/// </summary>
	public class DebugMessageInfo {
		/// <summary>
		/// 测试运行器
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// 除错信息
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="runner">测试运行器</param>
		/// <param name="message">除错信息</param>
		public DebugMessageInfo(TestRunner runner, string message) {
			Runner = runner;
			Message = message;
		}
	}
}

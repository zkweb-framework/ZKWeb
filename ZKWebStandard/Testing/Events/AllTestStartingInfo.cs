namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// 运行所有测试前的信息
	/// </summary>
	public class AllTestStartingInfo {
		/// <summary>
		/// 测试运行器
		/// </summary>
		public TestRunner Runner { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="runner">测试运行器</param>
		public AllTestStartingInfo(TestRunner runner) {
			Runner = runner;
		}
	}
}

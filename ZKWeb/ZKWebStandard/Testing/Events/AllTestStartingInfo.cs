namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for all test starting<br/>
	/// 所有测试开始后的信息<br/>
	/// </summary>
	public class AllTestStartingInfo {
		/// <summary>
		/// Test runner<br/>
		/// 测试运行器<br/>
		/// </summary>
		public TestRunner Runner { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		public AllTestStartingInfo(TestRunner runner) {
			Runner = runner;
		}
	}
}

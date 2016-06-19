namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// 所有测试运行完毕后的信息
	/// </summary>
	public class AllTestCompletedInfo {
		/// <summary>
		/// 测试运行器
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// 测试结果的计数器
		/// </summary>
		public TestResultCounter Counter { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="runner">测试运行器</param>
		/// <param name="counter">测试结果的计数器</param>
		public AllTestCompletedInfo(TestRunner runner, TestResultCounter counter) {
			Runner = runner;
			Counter = counter;
		}
	}
}

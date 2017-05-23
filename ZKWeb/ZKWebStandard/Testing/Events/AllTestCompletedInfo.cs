namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for all test completed<br/>
	/// 所有测试完成后的信息<br/>
	/// </summary>
	public class AllTestCompletedInfo {
		/// <summary>
		/// Test runner<br/>
		/// 测试运行器<br/>
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Test result counter<br/>
		/// 测试结果计数器<br/>
		/// </summary>
		public TestResultCounter Counter { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="counter">Test result counter</param>
		public AllTestCompletedInfo(TestRunner runner, TestResultCounter counter) {
			Runner = runner;
			Counter = counter;
		}
	}
}

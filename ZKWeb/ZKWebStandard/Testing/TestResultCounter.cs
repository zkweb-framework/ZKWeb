namespace ZKWebStandard.Testing {
	/// <summary>
	/// Test result counter<br/>
	/// 测试结果计数器<br/>
	/// </summary>
	/// <seealso cref="TestRunner"/>
	public class TestResultCounter {
		/// <summary>
		/// The number of tests passed<br/>
		/// 通过的测试数量<br/>
		/// </summary>
		public ulong Passed { get; set; }
		/// <summary>
		/// The number of tests failed<br/>
		/// 失败的测试数量<br/>
		/// </summary>
		public ulong Failed { get; set; }
		/// <summary>
		/// The number of tests skipped<br/>
		/// 跳过的测试数量<br/>
		/// </summary>
		public ulong Skipped { get; set; }
	}
}

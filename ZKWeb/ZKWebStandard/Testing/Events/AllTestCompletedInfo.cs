namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for all test completed<br/>
	/// <br/>
	/// </summary>
	public class AllTestCompletedInfo {
		/// <summary>
		/// Test runner<br/>
		/// <br/>
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Test result counter<br/>
		/// <br/>
		/// </summary>
		public TestResultCounter Counter { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="counter">Test result counter</param>
		public AllTestCompletedInfo(TestRunner runner, TestResultCounter counter) {
			Runner = runner;
			Counter = counter;
		}
	}
}

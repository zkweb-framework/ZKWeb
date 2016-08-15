namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for all test completed
	/// </summary>
	public class AllTestCompletedInfo {
		/// <summary>
		/// Test runner
		/// </summary>
		public TestRunner Runner { get; private set; }
		/// <summary>
		/// Test result counter
		/// </summary>
		public TestResultCounter Counter { get; private set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="runner">Test runner</param>
		/// <param name="counter">Test result counter</param>
		public AllTestCompletedInfo(TestRunner runner, TestResultCounter counter) {
			Runner = runner;
			Counter = counter;
		}
	}
}

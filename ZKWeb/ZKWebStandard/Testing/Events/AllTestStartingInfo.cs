namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for all test starting
	/// </summary>
	public class AllTestStartingInfo {
		/// <summary>
		/// Test runner
		/// </summary>
		public TestRunner Runner { get; private set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="runner">Test runner</param>
		public AllTestStartingInfo(TestRunner runner) {
			Runner = runner;
		}
	}
}

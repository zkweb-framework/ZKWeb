namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Information for all test starting<br/>
	/// <br/>
	/// </summary>
	public class AllTestStartingInfo {
		/// <summary>
		/// Test runner<br/>
		/// <br/>
		/// </summary>
		public TestRunner Runner { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="runner">Test runner</param>
		public AllTestStartingInfo(TestRunner runner) {
			Runner = runner;
		}
	}
}

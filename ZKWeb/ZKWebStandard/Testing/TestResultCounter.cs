namespace ZKWebStandard.Testing {
	/// <summary>
	/// Test result counter
	/// </summary>
	public class TestResultCounter {
		/// <summary>
		/// The number of tests passed
		/// </summary>
		public ulong Passed { get; set; }
		/// <summary>
		/// The number of tests failed
		/// </summary>
		public ulong Failed { get; set; }
		/// <summary>
		/// The number of tests skipped
		/// </summary>
		public ulong Skipped { get; set; }
	}
}

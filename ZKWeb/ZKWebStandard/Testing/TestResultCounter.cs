namespace ZKWebStandard.Testing {
	/// <summary>
	/// Test result counter<br/>
	/// <br/>
	/// </summary>
	public class TestResultCounter {
		/// <summary>
		/// The number of tests passed<br/>
		/// <br/>
		/// </summary>
		public ulong Passed { get; set; }
		/// <summary>
		/// The number of tests failed<br/>
		/// <br/>
		/// </summary>
		public ulong Failed { get; set; }
		/// <summary>
		/// The number of tests skipped<br/>
		/// <br/>
		/// </summary>
		public ulong Skipped { get; set; }
	}
}

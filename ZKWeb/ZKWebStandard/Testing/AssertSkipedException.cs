using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Assert skipped exception
	/// Will make the test skipped
	/// </summary>
	public class AssertSkipedException : Exception {
		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="reason">The reason</param>
		public AssertSkipedException(string reason) : base(reason) { }
	}
}

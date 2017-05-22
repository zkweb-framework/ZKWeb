using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Assert skipped exception<br/>
	/// Will make the test skipped<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	public class AssertSkipedException : Exception {
		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="reason">The reason</param>
		public AssertSkipedException(string reason) : base(reason) { }
	}
}

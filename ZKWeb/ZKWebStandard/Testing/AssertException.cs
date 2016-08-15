using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Assert failed exception
	/// </summary>
	public class AssertException : Exception {
		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="reason">The reason</param>
		public AssertException(string reason) : base(reason) { }
	}
}

using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Assert failed exception<br/>
	/// <br/>
	/// </summary>
	public class AssertException : Exception {
		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="reason">The reason</param>
		public AssertException(string reason) : base(reason) { }
	}
}

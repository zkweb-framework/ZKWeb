using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Assert failed exception<br/>
	/// 断言失败的例外<br/>
	/// </summary>
	/// <seealso cref="Assert"/>
	public class AssertException : Exception {
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="reason">The reason</param>
		public AssertException(string reason) : base(reason) { }
	}
}

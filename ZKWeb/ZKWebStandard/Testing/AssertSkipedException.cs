using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Assert skipped exception<br/>
	/// Will make the test skipped<br/>
	/// 断言跳过的例外<br/>
	/// 会跳过测试<br/>
	/// </summary>
	/// <seealso cref="Assert"/>
	public class AssertSkipedException : Exception {
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="reason">The reason</param>
		public AssertSkipedException(string reason) : base(reason) { }
	}
}

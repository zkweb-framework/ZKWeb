using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// 断言失败抛出的例外
	/// </summary>
	public class AssertException : Exception {
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="reason">失败原因</param>
		public AssertException(string reason) : base(reason) { }
	}
}

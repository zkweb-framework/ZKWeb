using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// 抛出此例外时
	/// 会把当前测试作为跳过处理
	/// </summary>
	public class AssertSkipedException : Exception {
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="reason">跳过原因</param>
		public AssertSkipedException(string reason) : base(reason) { }
	}
}

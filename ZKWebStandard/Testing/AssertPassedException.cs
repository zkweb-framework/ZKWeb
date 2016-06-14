using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// 抛出此例外时
	/// 会把当前测试作为通过处理
	/// </summary>
	public class AssertPassedException : Exception {
	}
}

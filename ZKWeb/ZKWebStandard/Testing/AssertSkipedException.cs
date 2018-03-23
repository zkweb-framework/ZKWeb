using System;

namespace ZKWebStandard.Testing {
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
	/// <summary>
	/// Assert skipped exception<br/>
	/// Will make the test skipped<br/>
	/// 断言跳过的例外<br/>
	/// 会跳过测试<br/>
	/// </summary>
	/// <seealso cref="Assert"/>
	public class AssertSkipedException : Exception {
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="reason">The reason</param>
		public AssertSkipedException(string reason) : base(reason) { }
	}
}

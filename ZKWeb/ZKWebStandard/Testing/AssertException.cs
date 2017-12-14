using System;

namespace ZKWebStandard.Testing {
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
	/// <summary>
	/// Assert failed exception<br/>
	/// 断言失败的例外<br/>
	/// </summary>
	/// <seealso cref="Assert"/>
	public class AssertException : Exception {
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="reason">The reason</param>
		public AssertException(string reason) : base(reason) { }
	}
}

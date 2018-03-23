using System;

namespace ZKWebStandard.Testing {
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
	/// <summary>
	/// Assert passed exception<br/>
	/// Will make the test passed<br/>
	/// 断言通过的例外<br/>
	/// 会让测试通过<br/>
	/// </summary>
	/// <seealso cref="Assert"/>
	public class AssertPassedException : Exception {
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
	}
}

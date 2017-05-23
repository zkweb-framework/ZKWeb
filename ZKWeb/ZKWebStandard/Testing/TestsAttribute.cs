using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Attribute for mark class contains test methods<br/>
	/// 标记类包含测试函数的属性<br/>
	/// </summary>
	/// <seealso cref="Assert"/>
	/// <seealso cref="TestRunner"/>
	/// <example>
	/// <code language="cs">
	/// [Tests]
	/// class ExampleTest {
	/// 	public void MethodA() {
	/// 		Assert.IsTrue(1 == 1);
	/// 		Assert.IsTrueWith(1 == 1, "if failed this item will be outputed");
	/// 		Assert.Equals(true, true);
	/// 		Assert.Throws&lt;ArgumentException&gt;(() =&gt; { throw new ArgumentException(); });
	/// 	}
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Class)]
	public class TestsAttribute : Attribute {
	}
}

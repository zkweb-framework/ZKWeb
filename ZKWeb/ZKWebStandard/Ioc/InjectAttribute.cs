using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Choose which constructor to inject explicitly<br/>
	/// 选择注入哪个构造函数时使用的属性<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	[AttributeUsage(AttributeTargets.Constructor)]
	public class InjectAttribute : Attribute {
	}
}

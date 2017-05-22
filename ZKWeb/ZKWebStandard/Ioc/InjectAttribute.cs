using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Choose which constructor to inject explicitly<br/>
	/// <br/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor)]
	public class InjectAttribute : Attribute {
	}
}

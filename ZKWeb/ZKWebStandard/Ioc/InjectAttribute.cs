using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Choose which constructor to inject explicitly
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor)]
	public class InjectAttribute : Attribute {
	}
}

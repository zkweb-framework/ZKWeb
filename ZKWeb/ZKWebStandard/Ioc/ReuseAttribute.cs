using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Reuse type attribute<br/>
	/// It should use with ExportManyAttributes<br/>
	/// 标记重用类型的属性<br/>
	/// 应该与ExportManyAttribute一起使用<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class ReuseAttribute : Attribute {
		/// <summary>
		/// Reuse type<br/>
		/// 重用类型<br/>
		/// </summary>
		public ReuseType ReuseType { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="reuseType">Reuse type</param>
		public ReuseAttribute(ReuseType reuseType) {
			ReuseType = reuseType;
		}
	}
}

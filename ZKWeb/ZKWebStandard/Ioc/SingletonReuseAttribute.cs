using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Singleton reuse attribute<br/>
	/// A convenient attribute from ReuseAttribute<br/>
	/// 标记单例的属性<br/>
	/// 继承了ReuseAttribute的便捷属性<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class SingletonReuseAttribute : ReuseAttribute {
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public SingletonReuseAttribute() : base(ReuseType.Singleton) { }
	}
}

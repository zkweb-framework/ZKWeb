using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Singleton reuse attribute
	/// A convenient attribute from ReuseAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class SingletonReuseAttribute : ReuseAttribute {
		/// <summary>
		/// Initialize
		/// </summary>
		public SingletonReuseAttribute() : base(ReuseType.Singleton) { }
	}
}

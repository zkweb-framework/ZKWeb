using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Singleton reuse attribute<br/>
	/// A convenient attribute from ReuseAttribute<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class SingletonReuseAttribute : ReuseAttribute {
		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		public SingletonReuseAttribute() : base(ReuseType.Singleton) { }
	}
}

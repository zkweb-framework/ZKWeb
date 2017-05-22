using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Reuse type attribute<br/>
	/// It should use with ExportManyAttributes<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class ReuseAttribute : Attribute {
		/// <summary>
		/// Reuse type<br/>
		/// <br/>
		/// </summary>
		public ReuseType ReuseType { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="reuseType">Reuse type</param>
		public ReuseAttribute(ReuseType reuseType) {
			ReuseType = reuseType;
		}
	}
}

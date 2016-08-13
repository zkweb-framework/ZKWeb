using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Reuse type attribute
	/// It should use with ExportManyAttributes
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class ReuseAttribute : Attribute {
		/// <summary>
		/// Reuse type
		/// </summary>
		public ReuseType ReuseType { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="reuseType">Reuse type</param>
		public ReuseAttribute(ReuseType reuseType) {
			ReuseType = reuseType;
		}
	}
}

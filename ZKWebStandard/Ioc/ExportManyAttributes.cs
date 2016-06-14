using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// 导出类型到自身类型和继承的类型
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct,
		Inherited = false,
		AllowMultiple = false)]
	public class ExportManyAttribute : Attribute {
		/// <summary>
		/// 关联键
		/// </summary>
		public object ContractKey { get; set; }
		/// <summary>
		/// 排除的类型列表
		/// </summary>
		public Type[] Except { get; set; }
		/// <summary>
		/// 是否包含私有类型，默认不包含
		/// </summary>
		public bool NonPublic { get; set; }
	}
}

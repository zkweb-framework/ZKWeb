using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Attribute for register type to IoC container with itself and it's base type and interfaces
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct,
		Inherited = false,
		AllowMultiple = false)]
	public class ExportManyAttribute : Attribute {
		/// <summary>
		/// Service key
		/// </summary>
		public object ContractKey { get; set; }
		/// <summary>
		/// Except types
		/// </summary>
		public Type[] Except { get; set; }
		/// <summary>
		/// Also register with non public service types
		/// </summary>
		public bool NonPublic { get; set; }
		/// <summary>
		/// Call Unregister first before register
		/// </summary>
		public bool ClearExists { get; set; }
	}
}

using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Attribute for register type to IoC container with itself and it's base type and interfaces<br/>
	/// <br/>
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct,
		Inherited = false,
		AllowMultiple = false)]
	public class ExportManyAttribute : Attribute {
		/// <summary>
		/// Service key<br/>
		/// <br/>
		/// </summary>
		public object ContractKey { get; set; }
		/// <summary>
		/// Except types<br/>
		/// <br/>
		/// </summary>
		public Type[] Except { get; set; }
		/// <summary>
		/// Also register with non public service types<br/>
		/// <br/>
		/// </summary>
		public bool NonPublic { get; set; }
		/// <summary>
		/// Unregister service types before register<br/>
		/// Please sure it won't unintentionally remove innocent implementations<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public bool ClearExists { get; set; }
	}
}

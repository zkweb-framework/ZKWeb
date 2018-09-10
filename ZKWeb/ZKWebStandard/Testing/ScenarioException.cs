using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Scenario exception used for for BDD<br/>
	/// 用于BDD的场景异常类<br/>
	/// </summary>
	public class ScenarioException : Exception {
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="reason">The reason</param>
		public ScenarioException(string reason) : base(reason) { }
	}
}
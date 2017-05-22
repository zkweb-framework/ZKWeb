namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Service reuse type<br/>
	/// <br/>
	/// </summary>
	public enum ReuseType {
		/// <summary>
		/// Transient, no reuse<br/>
		/// <br/>
		/// </summary>
		Transient = 0,
		/// <summary>
		/// Singleton, reuse in the future, it's granted to be thread safe<br/>
		/// <br/>
		/// </summary>
		Singleton = 1,
	}
}

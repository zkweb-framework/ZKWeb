namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Service reuse type
	/// </summary>
	public enum ReuseType {
		/// <summary>
		/// Transient, no reuse
		/// </summary>
		Transient = 0,
		/// <summary>
		/// Singleton, reuse in the future, it's granted to be thread safe
		/// </summary>
		Singleton = 1,
	}
}

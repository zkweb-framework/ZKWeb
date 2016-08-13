namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Action when service unresolved
	/// </summary>
	public enum IfUnresolved {
		/// <summary>
		/// Throw exception
		/// It's the default behaviour
		/// </summary>
		Throw = 0,
		/// <summary>
		/// Return default value
		/// </summary>
		ReturnDefault = 1
	}
}

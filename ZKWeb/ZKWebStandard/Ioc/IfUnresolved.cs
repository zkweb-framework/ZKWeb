namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Action when service unresolved<br/>
	/// <br/>
	/// </summary>
	public enum IfUnresolved {
		/// <summary>
		/// Throw exception<br/>
		/// It's the default behaviour<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		Throw = 0,
		/// <summary>
		/// Return default value<br/>
		/// </summary>
		ReturnDefault = 1
	}
}

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Action when service unresolved<br/>
	/// 服务解决失败时的操作<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	public enum IfUnresolved {
		/// <summary>
		/// Throw exception<br/>
		/// It's the default behaviour<br/>
		/// 抛出例外<br/>
		/// 它是默认的操作<br/>
		/// </summary>
		Throw = 0,
		/// <summary>
		/// Return default value<br/>
		/// 返回默认值<br/>
		/// </summary>
		ReturnDefault = 1
	}
}

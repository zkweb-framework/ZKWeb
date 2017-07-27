namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Service reuse type<br/>
	/// 重用类型<br/>
	/// </summary>
	/// <see cref="IContainer"/>
	/// <seealso cref="Container"/>
	public enum ReuseType {
		/// <summary>
		/// Transient, no reuse<br/>
		/// 临时，不重用<br/>
		/// </summary>
		Transient = 0,
		/// <summary>
		/// Singleton, reuse in the future, it's granted to be thread safe<br/>
		/// 单例，会在将来重用，并保证线程安全<br/>
		/// </summary>
		Singleton = 1,
		/// <summary>
		/// Scoped, reuse in the same scope, it's granted to be thread safe<br/>
		/// 区域, 会在同一个区域中重用, 并保证线程安全<br/>
		/// </summary>
		Scoped = 2
	}
}

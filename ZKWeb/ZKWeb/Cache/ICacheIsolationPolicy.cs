namespace ZKWeb.Cache {
	/// <summary>
	/// Cache isolation policy
	/// Require a key name when register to IoC container
	/// </summary>
	public interface ICacheIsolationPolicy {
		/// <summary>
		/// Get isolation key
		/// </summary>
		/// <returns></returns>
		object GetIsolationKey();
	}
}

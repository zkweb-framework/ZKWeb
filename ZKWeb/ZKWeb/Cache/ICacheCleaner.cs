namespace ZKWeb.Cache {
	/// <summary>
	/// Cache claner interface
	/// Usually for cleaning webpage and data cache
	/// </summary>
	public interface ICacheCleaner {
		/// <summary>
		/// Clean cache
		/// </summary>
		void ClearCache();
	}
}

namespace ZKWeb.Cache {
	/// <summary>
	/// The interface used to trigger the cache cleanup logic<br/>
	/// 用于触发缓存清理逻辑的接口<br/>
	/// </summary>
	/// <example>
	/// You can use the following code to free some memory<br/>
	/// 你可以使用以下的代码来释放一些内存<br/>
	/// <code language="cs">
	/// Application.Ioc.ResolveMany&lt;ICacheCleaner&gt;().ForEach(c => c.ClearCache());
	/// </code>
	/// </example>
	public interface ICacheCleaner {
		/// <summary>
		/// Clean cache<br/>
		/// 清理缓存<br/>
		/// </summary>
		void ClearCache();
	}
}

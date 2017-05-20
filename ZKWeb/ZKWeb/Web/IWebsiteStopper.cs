namespace ZKWeb.Web {
	/// <summary>
	/// Interface for stop website running<br/>
	/// Usually provide by hosting environment<br/>
	/// 用于停止网站运行的接口<br/>
	/// 通常由托管环境提供<br/>
	/// </summary>
	/// <seealso cref="Server.IApplication"/>
	/// <seealso cref="Plugin.PluginReloader"/>
	/// <example>
	/// <code language="cs">
	/// internal class AspNetWebsiteStopper : IWebsiteStopper {
	///		public void StopWebsite() {
	///			HttpRuntime.UnloadAppDomain();
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IWebsiteStopper {
		/// <summary>
		/// Stop website running<br/>
		/// 停止网站运行<br/>
		/// </summary>
		void StopWebsite();
	}
}

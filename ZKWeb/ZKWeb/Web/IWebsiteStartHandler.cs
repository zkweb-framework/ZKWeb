namespace ZKWeb.Web {
	/// <summary>
	/// Interface of website start handler<br/>
	/// Execute after all IPlugin executed<br/>
	/// 网站启动处理器的接口<br/>
	/// 在所有IPlugin执行后执行<br/>
	/// </summary>
	/// <seealso cref="Server.IApplication"/>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public ExampleWebsiteStartHandler : IWebsiteStartHandler {
	///		public void OnWebsiteStart() {
	///			var logManager = Application.Ioc.Resolve&lt;LogManager&gt;();
	///			logManager.LogDebug("website started");
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IWebsiteStartHandler {
		/// <summary>
		/// Handle website start<br/>
		/// 处理网站启动<br/>
		/// </summary>
		void OnWebsiteStart();
	}
}

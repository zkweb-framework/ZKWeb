namespace ZKWeb.Web {
	/// <summary>
	/// Interface for stop website running<br/>
	/// Usually provide by hosting environment<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="Server.IApplication"/>
	/// <seealso cref="Plugin.PluginReloader"/>
	public interface IWebsiteStopper {
		/// <summary>
		/// Stop website running<br/>
		/// <br/>
		/// </summary>
		void StopWebsite();
	}
}

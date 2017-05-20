namespace ZKWeb.Web {
	/// <summary>
	/// Interface for website start handler<br/>
	/// After all IPlugin called<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="Server.IApplication"/>
	public interface IWebsiteStartHandler {
		/// <summary>
		/// Handle website start<br/>
		/// <br/>
		/// </summary>
		void OnWebsiteStart();
	}
}

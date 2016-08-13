namespace ZKWeb.Web {
	/// <summary>
	/// Interface for website start handler
	/// After all IPlugin called
	/// </summary>
	public interface IWebsiteStartHandler {
		/// <summary>
		/// Handle website start
		/// </summary>
		void OnWebsiteStart();
	}
}

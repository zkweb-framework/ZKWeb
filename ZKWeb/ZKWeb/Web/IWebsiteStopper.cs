namespace ZKWeb.Web {
	/// <summary>
	/// Interface for stop website running
	/// Usually provide by hosting environment
	/// </summary>
	public interface IWebsiteStopper {
		/// <summary>
		/// Stop website running
		/// </summary>
		void StopWebsite();
	}
}

using System.Web;
using ZKWeb.Web;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Owin website stopper
	/// </summary>
	public class OwinWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// Unload app domain
		/// </summary>
		public void StopWebsite() {
			// Owin didn't provide interface for stopping website
			// Just use same method from Asp.Net
			HttpRuntime.UnloadAppDomain();
		}
	}
}

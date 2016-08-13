using System.Web;
using ZKWeb.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Asp.Net website stopper
	/// </summary>
	internal class AspNetWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// Unload app doamin
		/// </summary>
		public void StopWebsite() {
			HttpRuntime.UnloadAppDomain();
		}
	}
}

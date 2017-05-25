using System.Web;
using ZKWeb.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Asp.Net website stopper<br/>
	/// <br/>
	/// </summary>
	internal class AspNetWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// Unload app doamin<br/>
		/// <br/>
		/// </summary>
		public void StopWebsite() {
			HttpRuntime.UnloadAppDomain();
		}
	}
}

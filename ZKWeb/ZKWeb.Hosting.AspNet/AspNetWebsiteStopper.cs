using System.Web;
using ZKWeb.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Asp.Net website stopper<br/>
	/// Asp.Net网站停止器<br/>
	/// </summary>
	internal class AspNetWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// Unload app doamin<br/>
		/// 卸载AppDomain<br/>
		/// </summary>
		public void StopWebsite() {
			HttpRuntime.UnloadAppDomain();
		}
	}
}

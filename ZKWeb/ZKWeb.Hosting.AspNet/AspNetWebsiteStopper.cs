using System.Web;
using ZKWeb.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// 停止Asp.Net网站的运行
	/// </summary>
	internal class AspNetWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// 停止网站运行
		/// </summary>
		public void StopWebsite() {
			HttpRuntime.UnloadAppDomain();
		}
	}
}

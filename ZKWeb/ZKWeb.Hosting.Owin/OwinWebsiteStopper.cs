using System.Web;
using ZKWeb.Web;

namespace ZKWeb.Owin.Hosting {
	/// <summary>
	/// 停止Owin网站的运行
	/// </summary>
	public class OwinWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// 停止网站运行
		/// </summary>
		public void StopWebsite() {
			// Owin没有提供统一的接口停止网站，这里通过卸载程序域停止
			HttpRuntime.UnloadAppDomain();
		}
	}
}

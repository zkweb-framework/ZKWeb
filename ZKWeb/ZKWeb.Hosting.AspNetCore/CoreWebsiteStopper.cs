using Microsoft.AspNetCore.Hosting;
using ZKWeb.Web;
using ZKWebStandard.Ioc;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Asp.Net Core website stopper<br/>
	/// Asp.Net Core的网站停止器<br/>
	/// </summary>
	internal class CoreWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// Stop application<br/>
		/// 停止应用程序<br/>
		/// </summary>
		public void StopWebsite() {
			var lifetime = Application.Ioc.Resolve<IApplicationLifetime>(IfUnresolved.ReturnDefault);
			lifetime?.StopApplication();
		}
	}
}

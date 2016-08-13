using Microsoft.AspNetCore.Hosting;
using ZKWeb.Web;
using ZKWebStandard.Ioc;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Asp.Net Core website stopper
	/// </summary>
	internal class CoreWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// Stop application
		/// </summary>
		public void StopWebsite() {
			var lifetime = Application.Ioc.Resolve<IApplicationLifetime>(IfUnresolved.ReturnDefault);
			lifetime?.StopApplication();
		}
	}
}

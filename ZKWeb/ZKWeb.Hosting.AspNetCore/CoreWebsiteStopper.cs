using Microsoft.AspNetCore.Hosting;
using ZKWeb.Web;
using ZKWebStandard.Ioc;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Asp.Net Core website stopper<br/>
	/// <br/>
	/// </summary>
	internal class CoreWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// Stop application<br/>
		/// <br/>
		/// </summary>
		public void StopWebsite() {
			var lifetime = Application.Ioc.Resolve<IApplicationLifetime>(IfUnresolved.ReturnDefault);
			lifetime?.StopApplication();
		}
	}
}

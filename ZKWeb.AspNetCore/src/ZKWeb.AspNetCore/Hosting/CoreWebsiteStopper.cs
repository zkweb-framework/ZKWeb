using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ZKWeb.Web;
using ZKWebStandard.Ioc;

namespace ZKWeb.AspNetCore.Hosting {
	/// <summary>
	/// 停止Asp.Net Core网站的运行
	/// </summary>
	internal class CoreWebsiteStopper : IWebsiteStopper {
		/// <summary>
		/// 停止网站运行
		/// </summary>
		public void StopWebsite() {
			var host = Application.Ioc.Resolve<IWebHost>(IfUnresolved.ReturnDefault);
			if (host != null) {
				var lifetime = host.Services.GetService<IApplicationLifetime>();
				lifetime.StopApplication();
			}
		}
	}
}

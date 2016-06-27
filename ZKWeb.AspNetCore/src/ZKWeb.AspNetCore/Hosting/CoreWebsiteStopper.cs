using Microsoft.AspNetCore.Hosting;
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
			var lifetime = Application.Ioc.Resolve<IApplicationLifetime>(IfUnresolved.ReturnDefault);
			lifetime?.StopApplication();
		}
	}
}

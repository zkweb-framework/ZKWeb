using Owin;
using System.IO;
using System.Web;
using ZKWeb.Server;
using ZKWebStandard.Extensions;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Base startup class for owin<br/>
	/// Owin的启动类的基类<br/>
	/// </summary>
	public abstract class StartupBase : StartupBase<DefaultApplication> {

	}

	/// <summary>
	/// Base startup class for owin<br/>
	/// Owin的启动类的基类<br/>
	/// </summary>
	public abstract class StartupBase<TApplication>
		where TApplication : IApplication, new() {
		/// <summary>
		/// Get website root directory<br/>
		/// 获取网站根目录<br/>
		/// </summary>
		/// <returns></returns>
		public virtual string GetWebsiteRootDirectory() {
			if (HttpContext.Current != null) {
				return HttpContext.Current.Server.MapPath("~/");
			} else {
				return Directory.GetCurrentDirectory();
			}
		}

		/// <summary>
		/// Allow child class to configure other middlewares before zkweb middleware<br/>
		/// 允许子类配置其他在zkweb之前的中间件<br/>
		/// </summary>
		protected virtual void ConfigureMiddlewares(IAppBuilder app) { }

		/// <summary>
		/// Configure application<br/>
		/// 配置应用程序<br/>
		/// </summary>
		/// <param name="app">Owin application</param>
		public virtual void Configuration(IAppBuilder app) {
			// configure other middlewares
			ConfigureMiddlewares(app);
			// configure zkweb middleware
			var websiteRootDirectory = app.Properties.GetOrDefault<string>("host.WebsiteRootDirectory");
			websiteRootDirectory = websiteRootDirectory ?? GetWebsiteRootDirectory();
			app.UseZKWeb<TApplication>(GetWebsiteRootDirectory());
		}
	}
}

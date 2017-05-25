using Owin;
using System.IO;
using System.Web;
using ZKWeb.Server;
using ZKWebStandard.Extensions;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Base startup class for owin<br/>
	/// <br/>
	/// </summary>
	public abstract class StartupBase : StartupBase<DefaultApplication> {

	}

	/// <summary>
	/// Base startup class for owin<br/>
	/// <br/>
	/// </summary>
	public abstract class StartupBase<TApplication>
		where TApplication : IApplication, new() {
		/// <summary>
		/// Get website root directory<br/>
		/// <br/>
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
		/// Allow child class to configure middlewares<br/>
		/// <br/>
		/// </summary>
		protected virtual void ConfigureMiddlewares(IAppBuilder app) { }

		/// <summary>
		/// Configure owin application<br/>
		/// <br/>
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

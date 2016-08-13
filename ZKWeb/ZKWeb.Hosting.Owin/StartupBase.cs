using Owin;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using ZKWebStandard.Extensions;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Base startup class for owin
	/// </summary>
	public abstract class StartupBase {
		/// <summary>
		/// Get website root directory
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
		/// Allow child class to configure middlewares
		/// </summary>
		protected virtual void ConfigureMiddlewares(IAppBuilder app) { }

		/// <summary>
		/// Configure owin application
		/// </summary>
		/// <param name="app">Owin application</param>
		public virtual void Configuration(IAppBuilder app) {
			// Initialize application
			var websiteRootDirectory = app.Properties.GetOrDefault<string>("host.WebsiteRootDirectory");
			websiteRootDirectory = websiteRootDirectory ?? GetWebsiteRootDirectory();
			Application.Ioc.RegisterMany<OwinWebsiteStopper>();
			Application.Initialize(websiteRootDirectory);
			// Configure middlewares
			ConfigureMiddlewares(app);
			// Set request handler, it will running in thread pool
			// It can't throw any exception otherwise application will get killed
			app.Run(owinContext => Task.Run(() => {
				var context = new OwinHttpContextWrapper(owinContext);
				try {
					// Handle request
					Application.OnRequest(context);
				} catch (OwinHttpResponseEndException) {
					// Success
				} catch (Exception ex) {
					// Error
					try {
						Application.OnError(context, ex);
					} catch (OwinHttpResponseEndException) {
						// Handle error success
					} catch (Exception) {
						// Handle error failed
					}
				}
			}));
		}
	}
}

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ZKWeb;
using ZKWeb.Hosting.Owin;
using ZKWeb.Server;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace Owin {
	/// <summary>
	/// Owin application builder extension function<br/>
	/// Owin的应用构建器的扩展函数<br/>
	/// </summary>
	public static class OwinBuilderExtensions {
		/// <summary>
		/// Use zkweb middleware<br/>
		/// 使用zkweb中间件<br/>
		/// </summary>
		/// <param name="app">application builder</param>
		/// <param name="websiteRootDirectory">website root directory</param>
		/// <returns></returns>
		public static IAppBuilder UseZKWeb(
			this IAppBuilder app, string websiteRootDirectory) {
			return app.UseZKWeb<DefaultApplication>(websiteRootDirectory);
		}

		/// <summary>
		/// Use zkweb middleware<br/>
		/// 使用zkweb中间件<br/>
		/// </summary>
		/// <typeparam name="TApplication">application type</typeparam>
		/// <param name="app">application builder</param>
		/// <param name="websiteRootDirectory">website root directory</param>
		/// <returns></returns>
		[DebuggerNonUserCode]
		public static IAppBuilder UseZKWeb<TApplication>(
			this IAppBuilder app, string websiteRootDirectory)
			where TApplication : IApplication, new() {
			// set and initialize application
			Application.Initialize<TApplication>(websiteRootDirectory);
			Application.Ioc.RegisterMany<OwinWebsiteStopper>(ReuseType.Singleton);
			// set zkweb middleware
			// it can't throw any exception otherwise application will get killed
			return app.Use((coreContext, next) => Task.Run(() => {
				var context = new OwinHttpContextWrapper(coreContext);
				try {
					// Handle request
					Application.OnRequest(context);
				} catch (OwinHttpResponseEndException) {
					// Success
				} catch (Exception ex) {
					// Error
					if (ex is HttpException && ((HttpException)ex).StatusCode == 404) {
						// Try next middleware
						try {
							next().Wait();
							return;
						} catch (Exception nextEx) {
							ex = nextEx;
						}
					}
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

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ZKWeb;
using ZKWeb.Hosting.AspNetCore;
using ZKWeb.Server;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace Microsoft.AspNetCore.Builder {
	/// <summary>
	/// Asp.net Core application builder extension functions<br/>
	/// Asp.Net Core应用构建器的扩展函数<br/>
	/// </summary>
	public static class CoreBuilderExtensions {
		/// <summary>
		/// Use zkweb middleware<br/>
		/// 使用zkweb中间件<br/>
		/// </summary>
		[DebuggerNonUserCode]
		public static IApplicationBuilder UseZKWeb(
			this IApplicationBuilder app, string websiteRootDirectory) {
			return app.UseZKWeb<DefaultApplication>(websiteRootDirectory);
		}

		/// <summary>
		/// Use zkweb middleware<br/>
		/// 使用zkweb中间件<br/>
		/// </summary>
		[DebuggerNonUserCode]
		public static IApplicationBuilder UseZKWeb<TApplication>(
			this IApplicationBuilder app, string websiteRootDirectory)
			where TApplication : IApplication, new() {
			// Set and initialize application
			Application.Initialize<TApplication>(websiteRootDirectory);
			Application.Ioc.RegisterMany<CoreWebsiteStopper>(ReuseType.Singleton);
			// It can't throw any exception otherwise application will get killed
			return app.Use((coreContext, next) => Task.Run(() => {
				var context = new CoreHttpContextWrapper(coreContext);
				try {
					// Handle request
					Application.OnRequest(context);
				} catch (CoreHttpResponseEndException) {
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
					} catch (CoreHttpResponseEndException) {
						// Handle error success
					} catch (Exception) {
						// Handle error failed
					}
				}
			}));
		}
	}
}

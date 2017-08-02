using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ZKWeb;
using ZKWeb.Hosting.AspNetCore;
using ZKWeb.Server;
using ZKWebStandard.Ioc;
using ZKWebStandard.Ioc.Extensions;
using ZKWebStandard.Web;

namespace Microsoft.AspNetCore.Builder {
	/// <summary>
	/// Asp.net Core application builder extension functions<br/>
	/// Asp.Net Core应用构建器的扩展函数<br/>
	/// </summary>
	public static class CoreBuilderExtensions {
		/// <summary>
		/// Add zkweb services<br/>
		/// 添加ZKWeb服务<br/>
		/// </summary>
		public static IServiceProvider AddZKWeb(
			this IServiceCollection services, string websiteRootDirectory) {
			return services.AddZKWeb<DefaultApplication>(websiteRootDirectory);
		}

		/// <summary>
		/// Add zkweb services<br/>
		/// 添加ZKWeb服务<br/>
		/// </summary>
		public static IServiceProvider AddZKWeb<TApplication>(
			this IServiceCollection services, string websiteRootDirectory)
			where TApplication : IApplication, new() {
			Application.Initialize<TApplication>(websiteRootDirectory);
			Application.Ioc.RegisterMany<CoreWebsiteStopper>(ReuseType.Singleton);
			Application.Ioc.RegisterFromServiceCollection(services);
			return Application.Ioc.AsServiceProvider();
		}

		/// <summary>
		/// Use zkweb middleware<br/>
		/// 使用zkweb中间件<br/>
		/// </summary>
		[DebuggerNonUserCode]
		public static IApplicationBuilder UseZKWeb(this IApplicationBuilder app) {
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

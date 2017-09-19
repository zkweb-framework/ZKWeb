using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ZKWeb.Server;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Base startup class for Asp.Net Core<br/>
	/// Asp.Net Core的启动类的基类<br/>
	/// </summary>
	public abstract class StartupBase : StartupBase<DefaultApplication> {

	}

	/// <summary>
	/// Base startup class for Asp.Net Core<br/>
	/// Asp.Net Core的启动类的基类<br/>
	/// </summary>
	public abstract class StartupBase<TApplication>
		where TApplication : IApplication, new() {
		/// <summary>
		/// Stop application after error reported to browser needs some delay<br/>
		/// 在网站报告错误给浏览器后停止网站需要延迟一段时间<br/>
		/// </summary>
		public const int StopApplicationDelay = 3000;

		/// <summary>
		/// Get website root directory<br/>
		/// 获取网站根目录<br/>
		/// </summary>
		/// <returns></returns>
		protected virtual string GetWebsiteRootDirectory() {
			var path = PlatformServices.Default.Application.ApplicationBasePath;
			while (!(File.Exists(Path.Combine(path, "Web.config")) ||
				File.Exists(Path.Combine(path, "web.config")))) {
				path = Path.GetDirectoryName(path);
				if (string.IsNullOrEmpty(path)) {
					throw new DirectoryNotFoundException("Website root directory not found");
				}
			}
			return path;
		}

		/// <summary>
		/// Stop application after error reported to browser<br/>
		/// 在网站报告错误给浏览器后停止网站(需要延迟一段时间)<br/>
		/// </summary>
		protected virtual void StopApplicationAfter(IServiceProvider serviceProvider, int milliseconds) {
			var lifetime = (IApplicationLifetime)serviceProvider.GetService(typeof(IApplicationLifetime));
			var thread = new Thread(() => {
				Thread.Sleep(milliseconds);
				lifetime.StopApplication();
				// not always work, do the failback
				Environment.Exit(0);
				Process.GetCurrentProcess().Kill();
			});
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>
		/// Configure other middlewares before zkweb middleware<br/>
		/// 配置在zkweb之前的中间件<br/>
		/// </summary>
		protected virtual void ConfigureMiddlewares(IApplicationBuilder app) { }

		/// <summary>
		/// Configure other services before zkweb services<br/>
		/// 配置在zkweb之前的服务<br/>
		/// </summary>
		protected virtual void ConfigureOtherServices(IServiceCollection services) { }

		/// <summary>
		/// Configure services for IoC container<br/>
		/// 配置IoC容器的服务<br/>
		/// </summary>
		public virtual IServiceProvider ConfigureServices(IServiceCollection services) {
			try {
				// Configure other services
				ConfigureOtherServices(services);
				// Configure zkweb services
				return services.AddZKWeb<TApplication>(GetWebsiteRootDirectory());
			} catch {
				// Stop application after error reported to browser
				var serviceProvider = services.BuildServiceProvider();
				StopApplicationAfter(serviceProvider, StopApplicationDelay);
				throw;
			}
		}

		/// <summary>
		/// Configure application<br/>
		/// 配置应用程序<br/>
		/// </summary>
		public virtual void Configure(IApplicationBuilder app) {
			try {
				// Configure other middlewares
				ConfigureMiddlewares(app);
				// Configure zkweb middleware
				app.UseZKWeb();
			} catch {
				// Stop application after error reported to browser
				StopApplicationAfter(app.ApplicationServices, StopApplicationDelay);
				throw;
			}
		}
	}
}

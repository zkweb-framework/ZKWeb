using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Threading;
using ZKWeb.Server;
using ZKWebStandard.Extensions;

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
		/// 在网站报告错误给浏览器后停止网站 (延迟一段时间)<br/>
		/// </summary>
		protected virtual void StopApplicationAfter(IApplicationBuilder app, int milliseconds) {
			var lifetime = (IApplicationLifetime)app.ApplicationServices.GetService(typeof(IApplicationLifetime));
			var thread = new Thread(() => { Thread.Sleep(milliseconds); lifetime.StopApplication(); });
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>
		/// Allow child class to configure other middlewares before zkweb middleware<br/>
		/// 允许子类配置其他在zkweb之前的中间件<br/>
		/// </summary>
		protected virtual void ConfigureMiddlewares(IApplicationBuilder app) { }

		/// <summary>
		/// Configure services for IoC container<br/>
		/// 配置IoC容器的服务<br/>
		/// </summary>
		public virtual IServiceProvider ConfigureServices(IServiceCollection services) {
			Application.Ioc.RegisterFromServiceCollection(services);
			return Application.Ioc.AsServiceProvider();
		}

		/// <summary>
		/// Configure application<br/>
		/// 配置应用程序<br/>
		/// </summary>
		public virtual void Configure(IApplicationBuilder app) {
			try {
				// configure other middlewares
				ConfigureMiddlewares(app);
				// configure zkweb middleware
				app.UseZKWeb<TApplication>(GetWebsiteRootDirectory());
			} catch {
				// stop application after error reported to browser
				StopApplicationAfter(app, 3000);
				throw;
			}
		}
	}
}

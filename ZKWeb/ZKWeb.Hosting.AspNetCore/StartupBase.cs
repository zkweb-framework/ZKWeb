using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZKWebStandard.Ioc;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Base startup class for Asp.Net Core
	/// </summary>
	public abstract class StartupBase {
		/// <summary>
		/// Get website root directory
		/// </summary>
		/// <returns></returns>
		public virtual string GetWebsiteRootDirectory() {
			var path = PlatformServices.Default.Application.ApplicationBasePath;
			while (!File.Exists(Path.Combine(path, "Web.config"))) {
				path = Path.GetDirectoryName(path);
				if (string.IsNullOrEmpty(path)) {
					throw new DirectoryNotFoundException("Website root directory not found");
				}
			}
			return path;
		}

		/// <summary>
		/// Allow child class to configure middlewares
		/// </summary>
		protected virtual void ConfigureMiddlewares(IApplicationBuilder app) { }

		/// <summary>
		/// Configure application
		/// </summary>
		public virtual void Configure(IApplicationBuilder app, IApplicationLifetime lifetime) {
			try {
				// Initialize application
				Application.Ioc.RegisterMany<CoreWebsiteStopper>(ReuseType.Singleton);
				Application.Initialize(GetWebsiteRootDirectory());
				Application.Ioc.RegisterInstance(lifetime);
				// Configure middlewares
				ConfigureMiddlewares(app);
			} catch {
				// Stop application after error reported
				var thread = new Thread(() => { Thread.Sleep(3000); lifetime.StopApplication(); });
				thread.IsBackground = true;
				thread.Start();
				throw;
			}
			// Set request handler, it will running in thread pool
			// It can't throw any exception otherwise application will get killed
			app.Run(coreContext => Task.Run(() => {
				var context = new CoreHttpContextWrapper(coreContext);
				try {
					// Handle request
					Application.OnRequest(context);
				} catch (CoreHttpResponseEndException) {
					// Success
				} catch (Exception ex) {
					// Error
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Threading;
using ZKWeb.Server;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Base startup class for Asp.Net Core<br/>
	/// <br/>
	/// </summary>
	public abstract class StartupBase : StartupBase<DefaultApplication> {

	}

	/// <summary>
	/// Base startup class for Asp.Net Core<br/>
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
		/// <br/>
		/// </summary>
		protected virtual void StopApplicationAfter(IApplicationBuilder app, int milliseconds) {
			var lifetime = (IApplicationLifetime)app.ApplicationServices.GetService(typeof(IApplicationLifetime));
			var thread = new Thread(() => { Thread.Sleep(milliseconds); lifetime.StopApplication(); });
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>
		/// Allow child class to configure other middlewares before zkweb middleware<br/>
		/// <br/>
		/// </summary>
		protected virtual void ConfigureMiddlewares(IApplicationBuilder app) { }

		/// <summary>
		/// Configure application<br/>
		/// <br/>
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

using System;
using System.Threading;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Base application class for Asp.Net
	/// </summary>
	public abstract class GlobalBase : System.Web.HttpApplication {
		/// <summary>
		/// On website start
		/// </summary>
		protected virtual void Application_Start(object sender, EventArgs e) {
			ZKWeb.Application.Ioc.RegisterMany<AspNetWebsiteStopper>();
			ZKWeb.Application.Initialize(Server.MapPath("~/"));
		}

		/// <summary>
		/// On http request
		/// </summary>
		protected virtual void Application_BeginRequest(object sender, EventArgs e) {
			var context = new AspNetHttpContextWrapper(Context);
			try {
				// Handle http request
				ZKWeb.Application.OnRequest(context);
			} catch (ThreadAbortException) {
				// Success
				// Throw the original exception
				// Attention: original exception is a iis wrapped thread abort exception
				throw;
			} catch (Exception ex) {
				// Error
				ZKWeb.Application.OnError(context, ex);
			}
		}
	}
}

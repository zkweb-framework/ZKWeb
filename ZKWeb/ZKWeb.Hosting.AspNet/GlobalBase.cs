using System;
using System.Diagnostics;
using System.Threading;
using ZKWeb.Server;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Base application class for Asp.Net<br/>
	/// Asp.Net的基础应用类<br/>
	/// </summary>
	public abstract class GlobalBase : GlobalBase<DefaultApplication> {

	}

	/// <summary>
	/// Base application class for Asp.Net<br/>
	/// Asp.Net的基础应用类<br/>
	/// </summary>
	/// <typeparam name="TApplication">Application type</typeparam>
	public abstract class GlobalBase<TApplication> : System.Web.HttpApplication
		where TApplication : IApplication, new() {
		/// <summary>
		/// On website start<br/>
		/// 网站启动时的处理<br/>
		/// </summary>
		protected virtual void Application_Start(object sender, EventArgs e) {
			ZKWeb.Application.Initialize<TApplication>(Server.MapPath("~/"));
			ZKWeb.Application.Ioc.RegisterMany<AspNetWebsiteStopper>();
		}

		/// <summary>
		/// On http request<br/>
		/// 收到Http请求时的处理<br/>
		/// </summary>
		[DebuggerNonUserCode]
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

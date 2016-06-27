using System;
using System.Threading;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Asp.Net全局处理器的基类
	/// </summary>
	public abstract class GlobalBase : System.Web.HttpApplication {
		/// <summary>
		/// 网站启动时的处理
		/// </summary>
		protected virtual void Application_Start(object sender, EventArgs e) {
			ZKWeb.Application.Ioc.RegisterMany<AspNetWebsiteStopper>();
			ZKWeb.Application.Initialize(Server.MapPath("~/"));
		}

		/// <summary>
		/// 处理Http请求
		/// </summary>
		protected virtual void Application_BeginRequest(object sender, EventArgs e) {
			var context = new AspNetHttpContextWrapper(Context);
			try {
				// 处理请求
				ZKWeb.Application.OnRequest(context);
			} catch (ThreadAbortException) {
				// 正常处理完毕
				// 这里需要throw的原因
				// - IIS抛出的线程终止错误是包装的类型，表示请求结束
				// - 如果让运行库抛出原本的例外会导致IIS认为不是请求结束而是程序结束
				throw;
			} catch (Exception ex) {
				// 处理错误
				ZKWeb.Application.OnError(context, ex);
			}
		}
	}
}

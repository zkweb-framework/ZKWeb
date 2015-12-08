using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using ZKWeb.Core;
using ZKWeb.Core.Model;
using DryIoc;
using ZKWeb.Manager;
using ZKWeb.Model;
using ZKWeb.Utils.Extensions;
using ZKWeb.Properties;

namespace ZKWeb {
	/// <summary>
	/// 网站程序
	/// 用于初始化网站和保存全局数据
	/// 
	/// TODO:
	/// log monitor
	/// database migrate
	/// plugin break point test
	/// template engine
	/// session, cookies, csrf manager
	/// </summary>
	public class Application : HttpApplication {
		/// <summary>
		/// 当前使用的网站程序对象
		/// </summary>
		public static Application Current { get; set; }
		/// <summary>
		/// Ioc容器
		/// </summary>
		public Container Ioc { get; set; } = new Container();

		/// <summary>
		/// 网站启动时的处理
		/// </summary>
		public void Application_Start() {
			Current = this;
			Ioc.RegisterMany<ConfigManager>(Reuse.Singleton);
			Ioc.RegisterMany<LogManager>(Reuse.Singleton);
			Ioc.RegisterMany<PluginManager>(Reuse.Singleton);
			Ioc.Resolve<PluginManager>();
			Reloader.Start(this);
		}

		/// <summary>
		/// 收到Http请求时的处理
		/// </summary>
		protected void Application_BeginRequest(object sender, EventArgs e) {
			Response.Write("hello world");
			Response.End();
		}

		/// <summary>
		/// 捕获到例外时的处理
		/// </summary>
		protected void Application_Error(object sender, EventArgs e) {
			// 获取并清理最后抛出的例外
			// 对于Application_Start抛出的例外访问Response时会出错，这时需要记录到错误日志中并等待重试
			var ex = Server.GetLastError();
			if (ex is HttpUnhandledException && ex.InnerException != null) {
				ex = ex.InnerException;
			}
			Server.ClearError();
			try {
				Response.Clear();
			} catch {
				new LogManager().LogError(ex.ToString());
				HttpRuntime.UnloadAppDomain();
				return;
			}
			// 记录到日志
			// 不记录404（找不到）和403（权限不足）错误
			var logManager = Ioc.Resolve<LogManager>();
			var httpException = ex as HttpException;
			if (!(httpException?.GetHttpCode() == 404 ||
				httpException?.GetHttpCode() == 403)) {
				logManager.LogError(ex.ToString());
			}
			// 调用回调处理错误信息
			// 如回调中重定向或结束请求的处理，会抛出ThreadAbortException
			var handlers = Ioc.ResolveMany<IApplicationErrorHandler>();
			handlers.ForEach(h => h.OnError(ex));
			// 回调没有结束处理是，显示默认的信息
			// 错误是程序错误，且请求来源是本地时显示具体的信息
			// 让IE显示自定义错误需要有足够的长度，这里只能在后面填充空白内容
			bool isAjaxRequest = Request.IsAjaxRequest();
			if (httpException?.GetHttpCode() == 404) {
				Response.StatusCode = 404;
				Response.ContentType = "text/html";
				Response.Write(Resources._404NotFound);
				if (!isAjaxRequest) {
					Response.Write(Resources.HistoryBackScript);
				}
				Response.Write(string.Concat(Enumerable.Repeat("<div></div>", 100)));
			} else if (httpException?.GetHttpCode() == 403) {
				Response.StatusCode = 403;
				Response.ContentType = "text/html";
				Response.Write(httpException.Message);
				if (!isAjaxRequest) {
					Response.Write(Resources.HistoryBackScript);
				}
				Response.Write(string.Concat(Enumerable.Repeat("<div></div>", 100)));
			} else {
				Response.StatusCode = 500;
				Response.ContentType = "text/plain";
				Response.Write(Resources._500ServerInternalError);
				if (Request.IsLocal) {
					Response.Write($"\r\n{Resources.DisplayApplicationErrorDetails}\r\n\r\n");
					Response.Write(ex.ToString());
				} else {
					Response.Write(string.Concat(Enumerable.Repeat("\r\n", 100)));
				}
			}
			Response.End();
		}
	}
}

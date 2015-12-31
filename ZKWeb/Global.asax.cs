using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using ZKWeb.Core;
using DryIoc;
using ZKWeb.Model;
using ZKWeb.Utils.Extensions;
using ZKWeb.Properties;

namespace ZKWeb {
	/// <summary>
	/// 网站程序
	/// 用于初始化网站和保存全局数据
	/// </summary>
	public class Application : HttpApplication {
		/// <summary>
		/// 全局使用的Ioc容器
		/// </summary>
		public static Container Ioc { get; set; } = new Container();

		/// <summary>
		/// 网站启动时的处理
		/// </summary>
		public void Application_Start() {
			// 注册管理器类型
			Ioc.RegisterMany<ConfigManager>(Reuse.Singleton);
			Ioc.RegisterMany<ControllerManager>(Reuse.Singleton);
			Ioc.RegisterMany<DatabaseManager>(Reuse.Singleton);
			Ioc.RegisterMany<DiyManager>(Reuse.Singleton);
			Ioc.RegisterMany<InitJsonSettings>(Reuse.Singleton);
			Ioc.RegisterMany<LogManager>(Reuse.Singleton);
			Ioc.RegisterMany<PathManager>(Reuse.Singleton);
			Ioc.RegisterMany<PluginManager>(Reuse.Singleton);
			Ioc.RegisterMany<TemplateManager>(Reuse.Singleton);
			// 初始化管理器
			Ioc.Resolve<PluginManager>();
			Ioc.Resolve<TemplateManager>();
			Ioc.Resolve<ControllerManager>();
			Ioc.Resolve<InitJsonSettings>();
			Ioc.Resolve<DatabaseManager>();
			// 初始化所有插件
			Ioc.ResolveMany<IPlugin>().ForEach(p => { });
			// 自动重新载入插件和网站配置
			Reloader.Start();
		}

		/// <summary>
		/// 收到Http请求时的处理
		/// </summary>
		protected void Application_BeginRequest(object sender, EventArgs e) {
			var handlers = Ioc.ResolveMany<IHttpRequestHandler>();
			handlers.Reverse().ForEach(h => h.OnRequest()); // 后面注册的可以在前面处理
			throw new HttpException(404, "404 Not Found");
		}

		/// <summary>
		/// 捕获到例外时的处理
		/// 注意这个函数执行时使用的Application可能和初始化的不一样
		/// 获取Ioc成员时应该通过Current.Ioc
		/// </summary>
		protected void Application_Error(object sender, EventArgs e) {
			// 获取最后抛出的例外并记录到日志
			var ex = Server.GetLastError();
			if (ex is HttpUnhandledException && ex.InnerException != null) {
				ex = ex.InnerException;
			}
			// 记录到日志
			// 不记录400, 404, 403错误
			var logManager = Ioc.Resolve<LogManager>();
			var httpCode = (ex as HttpException)?.GetHttpCode();
			if (!(httpCode == 400 || httpCode == 404 || httpCode == 403)) {
				logManager.LogError(ex.ToString());
			}
			// 判断是否启动程序时抛出的错误，如果是则卸载程序域等待重试
			try {
				Response.StatusCode = Response.StatusCode;
			} catch {
				HttpRuntime.UnloadAppDomain();
				return;
			}
			// 调用回调处理错误
			// 如回调中重定向或结束请求的处理，会抛出ThreadAbortException
			var handlers = Ioc.ResolveMany<IHttpErrorHandler>();
			handlers.Reverse().ForEach(h => h.OnError(ex));
			// 如果是ajax请求则只返回例外消息
			if (Request.IsAjaxRequest()) {
				Response.StatusCode = httpCode ?? 500;
				Response.Write(ex.Message);
				Response.End();
			}
		}
	}
}

using System;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Extensions;
using ZKWeb.Database;
using ZKWeb.Web.Interfaces;
using ZKWeb.Plugin;
using ZKWeb.Logging;
using ZKWeb.Plugin.Interfaces;
using ZKWeb.Templating;
using ZKWeb.Serialize;
using ZKWeb.Web;
using ZKWeb.Server;
using ZKWeb.Templating.AreaSupport;
using ZKWeb.Localize.JsonConverters;
using ZKWeb.Localize;
using ZKWeb.UnitTest;
using System.Threading;
using ZKWeb.Utils.Collections;
using ZKWeb.Cache;
using ZKWeb.Cache.Policies;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Web.HttpRequestHandlers;

namespace ZKWeb {
	/// <summary>
	/// 网站主程序
	/// </summary>
	public class Application : HttpApplication {
		/// <summary>
		/// 框架的完整版本
		/// </summary>
		public static string FullVersion { get { return "0.9.6 testing"; } }
		/// <summary>
		/// 框架的数值版本
		/// </summary>
		public static Version Version { get { return Version.Parse(FullVersion.Split(' ')[0]); } }
		/// <summary>
		/// 当前使用的Ioc容器
		/// </summary>
		public static IContainer Ioc { get { return _overrideIoc.Value ?? _globalIoc; } }
		private static IContainer _globalIoc = new Container();
		private static ThreadLocal<IContainer> _overrideIoc = new ThreadLocal<IContainer>();

		/// <summary>
		/// 网站启动时的处理
		/// </summary>
		public void Application_Start() {
			// 注册核心组件
			Ioc.RegisterMany<DatabaseManager>(ReuseType.Singleton);
			Ioc.RegisterMany<TJsonConverter>(ReuseType.Singleton);
			Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
			Ioc.RegisterMany<LogManager>(ReuseType.Singleton);
			Ioc.RegisterMany<PluginManager>(ReuseType.Singleton);
			Ioc.RegisterMany<ConfigManager>(ReuseType.Singleton);
			Ioc.RegisterMany<PathConfig>(ReuseType.Singleton);
			Ioc.RegisterMany<PathManager>(ReuseType.Singleton);
			Ioc.RegisterMany<TemplateAreaManager>(ReuseType.Singleton);
			Ioc.RegisterMany<TemplateFileSystem>(ReuseType.Singleton);
			Ioc.RegisterMany<TemplateManager>(ReuseType.Singleton);
			Ioc.RegisterMany<UnitTestManager>(ReuseType.Singleton);
			Ioc.RegisterMany<AddVersionHeaderHandler>(ReuseType.Singleton);
			Ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
			Ioc.RegisterMany<CacheIsolateByDevice>(ReuseType.Singleton, serviceKey: "Device");
			Ioc.RegisterMany<CacheIsolateByLocale>(ReuseType.Singleton, serviceKey: "Locale");
			Ioc.RegisterMany<CacheIsolateByUrl>(ReuseType.Singleton, serviceKey: "Url");
			// 初始化核心组件
			ConfigManager.Initialize();
			PluginManager.Initialize();
			JsonNetInitializer.Initialize();
			TemplateManager.Initialize();
			ControllerManager.Initialize();
			DatabaseManager.Initialize();
			// 初始化所有插件并调用网站启动时的处理
			Ioc.ResolveMany<IPlugin>().ForEach(p => { });
			Ioc.ResolveMany<IWebsiteStartHandler>().ForEach(h => h.OnWebsiteStart());
			// 初始化常驻型的核心组件
			PluginReloader.Start();
			AutomaticCacheCleaner.Start();
		}

		/// <summary>
		/// 收到Http请求时的处理
		/// </summary>
		protected void Application_BeginRequest(object sender, EventArgs e) {
			// 调用预处理器，先注册的先调用
			foreach (var handler in Ioc.ResolveMany<IHttpRequestPreHandler>()) {
				handler.OnRequest();
			}
			// 调用处理器，后注册的先调用
			foreach (var handler in Ioc.ResolveMany<IHttpRequestHandler>().Reverse()) {
				handler.OnRequest();
			}
			// 没有处理时返回404
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
			// 不记录400~499之间的错误（客户端错误）
			var logManager = Ioc.Resolve<LogManager>();
			var httpCode = (ex as HttpException)?.GetHttpCode();
			if (!(httpCode >= 400 && httpCode < 500)) {
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
			var requestBase = new HttpRequestWrapper(Request);
			if (requestBase.IsAjaxRequest()) {
				Response.StatusCode = httpCode ?? 500;
				Response.Write(ex.Message);
				Response.End();
			}
		}

		/// <summary>
		/// 重载当前使用的Ioc容器，在当前线程中有效
		/// 重载后的容器会继承原有的容器，但不会对原有的容器做出修改
		/// </summary>
		/// <returns></returns>
		public static IDisposable OverrideIoc() {
			var previousOverride = _overrideIoc.Value;
			_overrideIoc.Value = (IContainer)Ioc.Clone();
			return new SimpleDisposable(() => {
				var tmp = _overrideIoc.Value;
				_overrideIoc.Value = previousOverride;
				tmp.Dispose();
			});
		}
	}
}

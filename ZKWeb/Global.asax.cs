using NHibernate.Util;
using System;
using System.Linq;
using System.Threading;
using ZKWeb.Cache;
using ZKWeb.Cache.Policies;
using ZKWeb.Database;
using ZKWeb.Localize;
using ZKWeb.Localize.JsonConverters;
using ZKWeb.Logging;
using ZKWeb.Plugin;
using ZKWeb.Serialize;
using ZKWeb.Server;
using ZKWeb.Templating;
using ZKWeb.Templating.DynamicContents;
using ZKWeb.Testing;
using ZKWeb.Web;
using ZKWeb.Web.HttpRequestHandlers;
using ZKWeb.Web.Wrappers;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace ZKWeb {
	/// <summary>
	/// 程序入口点
	/// </summary>
	public class Global : System.Web.HttpApplication {
		/// <summary>
		/// 初始化处理
		/// </summary>
		internal static void Initialize() {
			// 注册核心组件
			var ioc = ZKWeb.Application.Ioc;
			ioc.UnregisterAll();
			ioc.RegisterMany<DatabaseManager>(ReuseType.Singleton);
			ioc.RegisterMany<TJsonConverter>(ReuseType.Singleton);
			ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
			ioc.RegisterMany<LogManager>(ReuseType.Singleton);
			ioc.RegisterMany<PluginManager>(ReuseType.Singleton);
			ioc.RegisterMany<ConfigManager>(ReuseType.Singleton);
			ioc.RegisterMany<PathConfig>(ReuseType.Singleton);
			ioc.RegisterMany<PathManager>(ReuseType.Singleton);
			ioc.RegisterMany<TemplateAreaManager>(ReuseType.Singleton);
			ioc.RegisterMany<TemplateFileSystem>(ReuseType.Singleton);
			ioc.RegisterMany<TemplateManager>(ReuseType.Singleton);
			ioc.RegisterMany<TestManager>(ReuseType.Singleton);
			ioc.RegisterMany<AddVersionHeaderHandler>(ReuseType.Singleton);
			ioc.RegisterMany<DefaultErrorHandler>(ReuseType.Singleton);
			ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
			ioc.RegisterMany<CacheIsolateByDevice>(ReuseType.Singleton, serviceKey: "Device");
			ioc.RegisterMany<CacheIsolateByLocale>(ReuseType.Singleton, serviceKey: "Locale");
			ioc.RegisterMany<CacheIsolateByUrl>(ReuseType.Singleton, serviceKey: "Url");
			// 初始化核心组件
			ConfigManager.Initialize();
			PluginManager.Initialize();
			JsonNetInitializer.Initialize();
			TemplateManager.Initialize();
			ControllerManager.Initialize();
			DatabaseManager.Initialize();
			// 初始化所有插件并调用网站启动时的处理
			ioc.ResolveMany<IPlugin>().ForEach(p => { });
			ioc.ResolveMany<IWebsiteStartHandler>().ForEach(h => h.OnWebsiteStart());
			// 初始化常驻型的核心组件
			PluginReloader.Start();
			AutomaticCacheCleaner.Start();
			// 设置线程池使用尽可能多的线程
			// 实际本机设置的数量是(32767, 32767)
			ThreadPool.SetMaxThreads(int.MaxValue, int.MaxValue);
		}

		/// <summary>
		/// 网站启动时的处理
		/// </summary>
		protected void Application_Start(object sender, EventArgs e) {
			Initialize();
		}

		/// <summary>
		/// 处理Http请求
		/// </summary>
		protected void Application_BeginRequest(object sender, EventArgs e) {
			// 设置当前的Http上下文
			var context = new AspNetHttpContextWrapper(Context);
			using (HttpManager.OverrideContext(context)) {
				var ioc = ZKWeb.Application.Ioc;
				try {
					// 调用预处理器，先注册的先调用
					foreach (var handler in ioc.ResolveMany<IHttpRequestPreHandler>()) {
						handler.OnRequest();
					}
					// 调用处理器，后注册的先调用
					foreach (var handler in ioc.ResolveMany<IHttpRequestHandler>().Reverse()) {
						handler.OnRequest();
					}
					// 没有处理时返回404
					throw new HttpException(404, "Not Found");
				} catch (ThreadAbortException) {
					// 正常处理完毕
					// 这里需要throw的原因
					// - IIS抛出的线程终止错误是包装的类型，表示请求结束
					// - 如果让运行库抛出原本的例外会导致IIS认为不是请求结束而是程序结束
					throw;
				} catch (Exception ex) {
					// 调用错误处理器，后注册的先调用
					foreach (var handler in ioc.ResolveMany<IHttpRequestErrorHandler>()) {
						handler.OnError(ex);
					}
				}
			}
		}
	}
}

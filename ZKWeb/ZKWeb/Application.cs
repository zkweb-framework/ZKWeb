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
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace ZKWeb {
	/// <summary>
	/// 主程序
	/// </summary>
	public static class Application {
		/// <summary>
		/// 框架的完整版本
		/// </summary>
		public static string FullVersion { get { return "1.0.0 beta 3"; } }
		/// <summary>
		/// 框架的数值版本
		/// </summary>
		public static Version Version { get { return Version.Parse(FullVersion.Split(' ')[0]); } }
		/// <summary>
		/// 当前使用的容器
		/// 允许使用线程本地变量重载
		/// </summary>
		public static IContainer Ioc { get { return overrideIoc.Value ?? defaultIoc; } }
		private static IContainer defaultIoc = new Container();
		private static ThreadLocal<IContainer> overrideIoc = new ThreadLocal<IContainer>();
		/// <summary>
		/// 是否已初始化
		/// </summary>
		private static int Initialized = 0;

		/// <summary>
		/// 初始化主程序
		/// </summary>
		/// <param name="websiteRootDirectory">网站根目录的路径</param>
		public static void Initialize(string websiteRootDirectory) {
			// 重复初始化时抛出例外
			if (Interlocked.Exchange(ref Initialized, 1) != 0) {
				throw new ApplicationException("Application already initialized");
			}
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
			Ioc.RegisterMany<TestManager>(ReuseType.Singleton);
			Ioc.RegisterMany<AddVersionHeaderHandler>(ReuseType.Singleton);
			Ioc.RegisterMany<DefaultErrorHandler>(ReuseType.Singleton);
			Ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
			Ioc.RegisterMany<CacheIsolateByDevice>(ReuseType.Singleton, serviceKey: "Device");
			Ioc.RegisterMany<CacheIsolateByLocale>(ReuseType.Singleton, serviceKey: "Locale");
			Ioc.RegisterMany<CacheIsolateByUrl>(ReuseType.Singleton, serviceKey: "Url");
			// 初始化核心组件
			PathConfig.Initialize(websiteRootDirectory);
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
			// 设置线程池使用尽可能多的线程
			// 实际本机设置的数量是(32767, 32767)
			ThreadPool.SetMaxThreads(int.MaxValue, int.MaxValue);
		}

		/// <summary>
		/// 处理Http请求
		/// 处理完毕后会调用Response.End函数
		/// </summary>
		/// <param name="context">Http上下文</param>
		public static void OnRequest(IHttpContext context) {
			// 检测是否嵌套调用
			if (HttpManager.CurrentContextExists) {
				throw new ApplicationException("Nested call is unsupported");
			}
			// 调用请求处理器
			using (HttpManager.OverrideContext(context)) {
				// 调用预处理器，先注册的先调用
				foreach (var handler in Ioc.ResolveMany<IHttpRequestPreHandler>()) {
					handler.OnRequest();
				}
				// 调用处理器，后注册的先调用
				foreach (var handler in Ioc.ResolveMany<IHttpRequestHandler>().Reverse()) {
					handler.OnRequest();
				}
				// 没有处理时返回404
				throw new HttpException(404, "Not Found");
			}
		}

		/// <summary>
		/// 处理错误
		/// 处理完毕后会调用Response.End函数
		/// </summary>
		/// <param name="context">Http上下文</param>
		/// <param name="ex">错误对象</param>
		public static void OnError(IHttpContext context, Exception ex) {
			// 检测是否嵌套调用
			if (HttpManager.CurrentContextExists) {
				throw new ApplicationException("Nested call is unsupported");
			}
			// 调用错误处理器，后注册的先调用
			using (HttpManager.OverrideContext(context)) {
				foreach (var handler in Ioc.ResolveMany<IHttpRequestErrorHandler>()) {
					handler.OnError(ex);
				}
			}
		}

		/// <summary>
		/// 重载当前使用的Ioc容器，在当前线程中有效
		/// 重载后的容器会继承原有的容器，但不会对原有的容器做出修改
		/// </summary>
		/// <returns></returns>
		public static IDisposable OverrideIoc() {
			var previousOverride = overrideIoc.Value;
			overrideIoc.Value = (IContainer)Ioc.Clone();
			return new SimpleDisposable(() => {
				var tmp = overrideIoc.Value;
				overrideIoc.Value = previousOverride;
				tmp.Dispose();
			});
		}
	}
}

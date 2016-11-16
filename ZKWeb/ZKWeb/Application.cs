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
using ZKWeb.Plugin.AssemblyLoaders;
using ZKWeb.Plugin.CompilerServices;
using ZKWeb.Serialize;
using ZKWeb.Server;
using ZKWeb.Storage;
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
	/// Main Application
	/// </summary>
	public static class Application {
		/// <summary>
		/// ZKWeb Version String
		/// </summary>
		public static string FullVersion { get { return "1.2.1 beta 1"; } }
		/// <summary>
		/// ZKWeb Version Object
		/// </summary>
		public static Version Version { get { return Version.Parse(FullVersion.Split(' ')[0]); } }
		/// <summary>
		/// The IoC Container Instance
		/// Allow override by thread local variable
		/// </summary>
		public static IContainer Ioc { get { return overrideIoc.Value ?? defaultIoc; } }
		private static IContainer defaultIoc = new Container();
		private static ThreadLocal<IContainer> overrideIoc = new ThreadLocal<IContainer>();
		/// <summary>
		/// Initialize Flag
		/// </summary>
		private static int Initialized = 0;

		/// <summary>
		/// Intialize main application
		/// </summary>
		/// <param name="websiteRootDirectory">Website root directory</param>
		public static void Initialize(string websiteRootDirectory) {
			// Throw exception if already initialized
			if (Interlocked.Exchange(ref Initialized, 1) != 0) {
				throw new InvalidOperationException("Application already initialized");
			}
			// Register core components
			Ioc.RegisterMany<CacheFactory>(ReuseType.Singleton);
			Ioc.RegisterMany<DatabaseManager>(ReuseType.Singleton);
			Ioc.RegisterMany<TJsonConverter>(ReuseType.Singleton);
			Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
			Ioc.RegisterMany<LogManager>(ReuseType.Singleton);
#if NETCORE
			Ioc.RegisterMany<CoreAssemblyLoader>(ReuseType.Singleton);
#else
			Ioc.RegisterMany<NetAssemblyLoader>(ReuseType.Singleton);
#endif
			Ioc.RegisterMany<RoslynCompilerService>(ReuseType.Singleton);
			Ioc.RegisterMany<PluginManager>(ReuseType.Singleton);
#pragma warning disable CS0618
			Ioc.RegisterMany<ConfigManager>(ReuseType.Singleton);
			Ioc.RegisterMany<PathConfig>(ReuseType.Singleton);
			Ioc.RegisterMany<PathManager>(ReuseType.Singleton);
#pragma warning restore CS0618
			Ioc.RegisterMany<WebsiteConfigManager>(ReuseType.Singleton);
			Ioc.RegisterMany<LocalFileStorage>(ReuseType.Singleton);
			Ioc.RegisterMany<LocalPathConfig>(ReuseType.Singleton);
			Ioc.RegisterMany<LocalPathManager>(ReuseType.Singleton);
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
			// Initialize core components
			LocalPathConfig.Initialize(websiteRootDirectory);
			WebsiteConfigManager.Initialize();
			PluginManager.Initialize();
			JsonNetInitializer.Initialize();
			TemplateManager.Initialize();
			ThreadPoolInitializer.Initialize();
			DatabaseManager.Initialize();
			// Initialize all plugins and controllers
			Ioc.ResolveMany<IPlugin>().ForEach(p => { });
			ControllerManager.Initialize();
			Ioc.ResolveMany<IWebsiteStartHandler>().ForEach(h => h.OnWebsiteStart());
			// Start the resident core processes
			PluginReloader.Start();
			AutomaticCacheCleaner.Start();
		}

		/// <summary>
		/// Handle http request
		/// `Response.End` will be called if processing completed without errors
		/// </summary>
		/// <param name="context">Http context</param>
		public static void OnRequest(IHttpContext context) {
			// Detect nested call
			if (HttpManager.CurrentContextExists) {
				throw new InvalidOperationException("Nested call is unsupported");
			}
			// Call request handlers
			using (HttpManager.OverrideContext(context)) {
				// Call pre request handlers, in register order
				foreach (var handler in Ioc.ResolveMany<IHttpRequestPreHandler>()) {
					handler.OnRequest();
				}
				// Call request handlers, in reverse register order
				foreach (var handler in Ioc.ResolveMany<IHttpRequestHandler>().Reverse()) {
					handler.OnRequest();
				}
				// If request not get handled, throw an 404 exception
				throw new HttpException(404, "Not Found");
			}
		}

		/// <summary>
		/// Handle http error
		/// `Response.End` will be called if processing completed without errors
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="ex">Exception object</param>
		public static void OnError(IHttpContext context, Exception ex) {
			// Detect nested call
			if (HttpManager.CurrentContextExists) {
				throw new InvalidOperationException("Nested call is unsupported");
			}
			// Call error handlers, in reverse register order
			using (HttpManager.OverrideContext(context)) {
				foreach (var handler in
					Ioc.ResolveMany<IHttpRequestErrorHandler>().Reverse()) {
					handler.OnError(ex);
				}
			}
		}

		/// <summary>
		/// Override IoC container, only available for the thread calling this method
		/// Overrided container will inherit the original container,
		/// Alter overrided container will not affect the original container.
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

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
using ZKWeb.Storage;
using ZKWeb.Templating;
using ZKWeb.Templating.DynamicContents;
using ZKWeb.Testing;
using ZKWeb.Web;
using ZKWeb.Web.ActionParameterProviders;
using ZKWeb.Web.HttpRequestHandlers;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace ZKWeb.Server {
	/// <summary>
	/// Default application implementation
	/// </summary>
	public class DefaultApplication : IApplication {
		/// <summary>
		/// ZKWeb Version String
		/// </summary>
		public virtual string FullVersion { get { return "1.9.0 beta 1"; } }
		/// <summary>
		/// ZKWeb Version Object
		/// </summary>
		public virtual Version Version { get { return Version.Parse(FullVersion.Split(' ')[0]); } }
		/// <summary>
		/// The IoC Container Instance
		/// </summary>
		public virtual IContainer Ioc { get { return overrideIoc.Value ?? defaultIoc; } }
		/// <summary>
		/// Default IoC Container
		/// </summary>
		protected IContainer defaultIoc = new Container();
		/// <summary>
		/// Overrided IoC Container
		/// </summary>
		protected ThreadLocal<IContainer> overrideIoc = new ThreadLocal<IContainer>();
		/// <summary>
		/// In progress requests
		/// </summary>
		public virtual int InProgressRequests { get { return inProgressRequests; } }
		/// <summary>
		/// In progress requests
		/// </summary>
		protected int inProgressRequests = 0;
		/// <summary>
		/// Initialize Flag
		/// </summary>
		protected int initialized = 0;
		/// <summary>
		/// Website root directory
		/// </summary>
		protected string WebsiteRootDirectory { get; set; }

		/// <summary>
		/// Register components to the default container
		/// </summary>
		protected virtual void InitializeContainer() {
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
			Ioc.RegisterMany<TemplateWidgetRenderer>(ReuseType.Singleton);
			Ioc.RegisterMany<TemplateFileSystem>(ReuseType.Singleton);
			Ioc.RegisterMany<TemplateManager>(ReuseType.Singleton);
			Ioc.RegisterMany<TestManager>(ReuseType.Singleton);
			Ioc.RegisterMany<DefaultActionParameterProvider>(ReuseType.Singleton);
			Ioc.RegisterMany<AddVersionHeaderHandler>(ReuseType.Singleton);
			Ioc.RegisterMany<DefaultErrorHandler>(ReuseType.Singleton);
			Ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
			Ioc.RegisterMany<CacheIsolateByDevice>(ReuseType.Singleton, serviceKey: "Device");
			Ioc.RegisterMany<CacheIsolateByLocale>(ReuseType.Singleton, serviceKey: "Locale");
			Ioc.RegisterMany<CacheIsolateByUrl>(ReuseType.Singleton, serviceKey: "Url");
			Ioc.RegisterInstance<IApplication>(this);
		}

		/// <summary>
		/// Initialize core components
		/// </summary>
		protected virtual void InitializeCoreComponents() {
			LocalPathConfig.Initialize(WebsiteRootDirectory);
			WebsiteConfigManager.Initialize();
			PluginManager.Initialize();
			JsonNetInitializer.Initialize();
			TemplateManager.Initialize();
			ThreadPoolInitializer.Initialize();
			DatabaseManager.Initialize();
		}

		/// <summary>
		/// Initialize plugins
		/// </summary>
		protected virtual void InitializePlugins() {
			Ioc.ResolveMany<IPlugin>().ForEach(p => { });
			ControllerManager.Initialize();
			Ioc.ResolveMany<IWebsiteStartHandler>().ForEach(h => h.OnWebsiteStart());
		}

		/// <summary>
		/// Start core services
		/// </summary>
		protected virtual void StartServices() {
			PluginReloader.Start();
			AutomaticCacheCleaner.Start();
		}

		/// <summary>
		/// Intialize main application
		/// </summary>
		public virtual void Initialize(string websiteRootDirectory) {
			if (Interlocked.Exchange(ref initialized, 1) != 0) {
				throw new InvalidOperationException("Application already initialized");
			}
			try {
				WebsiteRootDirectory = websiteRootDirectory;
				InitializeContainer();
				InitializeCoreComponents();
				InitializePlugins();
				StartServices();
			} catch (Exception ex) {
				throw new Exception(ex.ToDetailedString(), ex);
			}
		}

		/// <summary>
		/// Handle http request
		/// </summary>
		public virtual void OnError(IHttpContext context, Exception ex) {
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
		/// Handle http error
		/// </summary>
		public virtual void OnRequest(IHttpContext context) {
			// Detect nested call
			if (HttpManager.CurrentContextExists) {
				throw new InvalidOperationException("Nested call is unsupported");
			}
			// Call request handlers
			using (HttpManager.OverrideContext(context)) {
				// Increase requests count
				Interlocked.Increment(ref inProgressRequests);
				try {
					// Call pre request handlers, in register order
					foreach (var handler in Ioc.ResolveMany<IHttpRequestPreHandler>()) {
						handler.OnRequest();
					}
					// Wrap handler action by wrappers
					var handlerAction = new Action(() => {
						// Call request handlers, in reverse register order
						foreach (var handler in Ioc.ResolveMany<IHttpRequestHandler>().Reverse()) {
							handler.OnRequest();
						}
						// If request not get handled, throw an 404 exception
						throw new HttpException(404, "Not Found");
					});
					foreach (var wrapper in Ioc.ResolveMany<IHttpRequestHandlerWrapper>()) {
						handlerAction = wrapper.WrapHandlerAction(handlerAction);
					}
					handlerAction();
				} finally {
					try {
						// Call post request handlers, in register order
						foreach (var handler in Ioc.ResolveMany<IHttpRequestPostHandler>()) {
							handler.OnRequest();
						}
					} finally {
						// Decrease requests count
						Interlocked.Decrement(ref inProgressRequests);
					}
				}
			}
		}

		/// <summary>
		/// Override IoC container
		/// </summary>
		public virtual IDisposable OverrideIoc() {
			var previousOverride = overrideIoc.Value;
			overrideIoc.Value = (IContainer)Ioc.Clone();
			return new SimpleDisposable(() => {
				var tmp = overrideIoc.Value;
				overrideIoc.Value = previousOverride;
				tmp?.Dispose();
			});
		}
	}
}

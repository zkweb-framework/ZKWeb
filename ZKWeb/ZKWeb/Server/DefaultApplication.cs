using System;
using System.IO;
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
using ZKWeb.Web.ActionCollections;
using ZKWeb.Web.ActionParameterProviders;
using ZKWeb.Web.HttpRequestHandlers;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace ZKWeb.Server {
	/// <summary>
	/// Default application implementation<br/>
	/// 默认应用类<br/>
	/// </summary>
	public class DefaultApplication : IApplication {
		/// <summary>
		/// ZKWeb Version String<br/>
		/// ZKWeb的版本字符串<br/>
		/// </summary>
		public virtual string FullVersion { get { return "2.0.0"; } }
		/// <summary>
		/// ZKWeb Version Object<br/>
		/// ZKWeb的版本对象<br/>
		/// </summary>
		public virtual Version Version { get { return Version.Parse(FullVersion.Split(' ')[0]); } }
		/// <summary>
		/// The IoC Container Instance<br/>
		/// IoC容器的实例<br/>
		/// </summary>
		public virtual IContainer Ioc { get { return overrideIoc.Value ?? defaultIoc; } }
		/// <summary>
		/// Default IoC Container
		/// 默认的IoC容器<br/>
		/// </summary>
		protected IContainer defaultIoc = new Container();
		/// <summary>
		/// Overrided IoC Container
		/// 在当前线程中重载的IoC容器<br/>
		/// </summary>
		protected ThreadLocal<IContainer> overrideIoc = new ThreadLocal<IContainer>();
		/// <summary>
		/// In progress requests<br/>
		/// 处理中的请求数量<br/>
		/// </summary>
		public virtual int InProgressRequests { get { return inProgressRequests; } }
		/// <summary>
		/// In progress requests<br/>
		/// 处理中的请求数量<br/>
		/// </summary>
		protected int inProgressRequests = 0;
		/// <summary>
		/// Initialize Flag<br/>
		/// 初始化标记<br/>
		/// </summary>
		protected int initialized = 0;
		/// <summary>
		/// Website root directory<br/>
		/// 网站根目录的路径<br/>
		/// </summary>
		protected string WebsiteRootDirectory { get; set; }

		/// <summary>
		/// Register components to the default container<br/>
		/// 注册组件到默认的容器<br/>
		/// </summary>
		protected virtual void InitializeContainer() {
			Ioc.RegisterMany<AutomaticCacheCleaner>(ReuseType.Singleton);
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
			Ioc.RegisterMany<PluginReloader>(ReuseType.Singleton);
			Ioc.RegisterMany<JsonNetInitializer>(ReuseType.Singleton);
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
			Ioc.RegisterMany<DefaultActionCollection>(ReuseType.Transient);
			Ioc.RegisterMany<DefaultActionParameterProvider>(ReuseType.Singleton);
			Ioc.RegisterMany<AddVersionHeaderHandler>(ReuseType.Singleton);
			Ioc.RegisterMany<DefaultErrorHandler>(ReuseType.Singleton);
			Ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
			Ioc.RegisterMany<ThreadPoolInitializer>(ReuseType.Singleton);
			Ioc.RegisterMany<CacheIsolateByDevice>(ReuseType.Singleton, serviceKey: "Device");
			Ioc.RegisterMany<CacheIsolateByLocale>(ReuseType.Singleton, serviceKey: "Locale");
			Ioc.RegisterMany<CacheIsolateByUrl>(ReuseType.Singleton, serviceKey: "Url");
			Ioc.RegisterInstance<IApplication>(this);
		}

		/// <summary>
		/// Initialize core components<br/>
		/// 初始化核心组件<br/>
		/// </summary>
		protected virtual void InitializeCoreComponents() {
			Ioc.Resolve<LocalPathConfig>().Initialize(WebsiteRootDirectory);
			Ioc.Resolve<WebsiteConfigManager>().Initialize();
			Ioc.Resolve<PluginManager>().Initialize();
			Ioc.Resolve<JsonNetInitializer>().Initialize();
			Ioc.Resolve<TemplateManager>().Initialize();
			Ioc.Resolve<ThreadPoolInitializer>().Initialize();
			Ioc.Resolve<DatabaseManager>().Initialize();
		}

		/// <summary>
		/// Initialize plugins<br/>
		/// 初始化插件<br/>
		/// </summary>
		protected virtual void InitializePlugins() {
			Ioc.ResolveMany<IPlugin>().ForEach(p => { });
			Ioc.Resolve<ControllerManager>().Initialize();
			Ioc.ResolveMany<IWebsiteStartHandler>().ForEach(h => h.OnWebsiteStart());
		}

		/// <summary>
		/// Start core services<br/>
		/// 开启核心服务<br/>
		/// </summary>
		protected virtual void StartServices() {
			Ioc.Resolve<PluginReloader>().Start();
			Ioc.Resolve<AutomaticCacheCleaner>().Start();
		}

		/// <summary>
		/// Log emergency error<br/>
		/// 记录紧急错误<br/>
		/// </summary>
		/// <param name="message"></param>
		protected virtual void LogEmergencyError(string message) {
			try {
				Console.WriteLine(message);
				var rootDirectory = WebsiteRootDirectory;
				if (string.IsNullOrEmpty(rootDirectory)) {
					rootDirectory = Path.GetTempPath();
				}
				var logPath = Path.Combine(rootDirectory, "emergencyError.log");
				File.AppendAllText(logPath, message + "\r\n");
			} catch {
			}
		}

		/// <summary>
		/// Intialize main application<br/>
		/// 初始化主应用<br/>
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
				var message = ex.ToDetailedString();
				LogEmergencyError(message);
				throw new Exception(message, ex);
			}
		}

		/// <summary>
		/// Handle http request<br/>
		/// 处理Http请求<br/>
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
		/// Handle http error<br/>
		/// 处理Http错误<br/>
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
						// Notify scope finished
						Ioc.ScopeFinished();
					}
				}
			}
		}

		/// <summary>
		/// Override IoC container<br/>
		/// 重载当前线程中的IoC容器<br/>
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

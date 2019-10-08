using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.FastReflection;
using System.Runtime.ExceptionServices;
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

namespace ZKWeb.Server
{
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
    /// <summary>
    /// Default application implementation<br/>
    /// 默认应用类<br/>
    /// </summary>
    public class DefaultApplication : IApplication, IDisposable
    {
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
        /// <summary>
        /// ZKWeb Version String<br/>
        /// ZKWeb的版本字符串<br/>
        /// </summary>
        public virtual string FullVersion { get { return _fullVersion; } }
        private readonly string _fullVersion = "";
        /// <summary>
        /// ZKWeb Version Object<br/>
        /// ZKWeb的版本对象<br/>
        /// </summary>
        public virtual Version Version { get { return Version.Parse(FullVersion.Split('-')[0]); } }
        /// <summary>
        /// The IoC Container Instance<br/>
        /// IoC容器的实例<br/>
        /// </summary>
        public virtual IContainer Ioc { get { return _overrideIoc.Value ?? _defaultIoc; } }
        /// <summary>
        /// Default IoC Container
        /// 默认的IoC容器<br/>
        /// </summary>
        protected IContainer _defaultIoc = new Container();
        /// <summary>
        /// Overrided IoC Container
        /// 在当前线程中重载的IoC容器<br/>
        /// </summary>
        protected ThreadLocal<IContainer> _overrideIoc = new ThreadLocal<IContainer>();
        /// <summary>
        /// In progress requests<br/>
        /// 处理中的请求数量<br/>
        /// </summary>
        public virtual int InProgressRequests { get { return _inProgressRequests; } }
        /// <summary>
        /// In progress requests<br/>
        /// 处理中的请求数量<br/>
        /// </summary>
        protected int _inProgressRequests;
        /// <summary>
        /// Initialize Flag<br/>
        /// 初始化标记<br/>
        /// </summary>
        protected int _initializeFlag;
        /// <summary>
        /// Ready Flag<br/>
        /// 可用标记<br/>
        /// </summary>
        protected volatile bool _readyFlag;
        /// <summary>
        /// The timeout for waiting application become ready<br/>
        /// 等待可用的超时<br/>
        /// </summary>
        protected TimeSpan _waitReadyTimeout = TimeSpan.FromSeconds(60);
        /// <summary>
        /// Exception occurs during initializing application<br/>
        /// 初始化过程中发生的异常<br/>
        /// </summary>
        protected volatile Exception _initializeException;
        /// <summary>
        /// Website root directory<br/>
        /// 网站根目录的路径<br/>
        /// </summary>
        protected string WebsiteRootDirectory { get; set; }

        /// <summary>
        /// Initialize<br/>
        /// 初始化<br/>
        /// </summary>
        public DefaultApplication()
        {
            _fullVersion = typeof(DefaultApplication).Assembly
                .GetCustomAttributes(true)
                .OfType<AssemblyInformationalVersionAttribute>().FirstOrDefault()
                ?.InformationalVersion
                ?.Trim() ?? "Unknown";
        }

        /// <summary>
        /// Register components to the default container<br/>
        /// 注册组件到默认的容器<br/>
        /// </summary>
        protected virtual void InitializeContainer()
        {
            Ioc.RegisterMany<AutomaticCacheCleaner>(ReuseType.Singleton);
            Ioc.RegisterMany<CacheFactory>(ReuseType.Singleton);
            Ioc.RegisterMany<DatabaseManager>(ReuseType.Singleton);
            Ioc.RegisterMany<TJsonConverter>(ReuseType.Singleton);
            Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
            Ioc.RegisterMany<LogManager>(ReuseType.Singleton);
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
        protected virtual void InitializeCoreComponents()
        {
            Ioc.Resolve<LocalPathConfig>().Initialize(WebsiteRootDirectory);
            Ioc.Resolve<WebsiteConfigManager>().Initialize();
#if NETCORE_3
            var config = Ioc.Resolve<WebsiteConfigManager>().WebsiteConfig;
            if (!config.Extra.GetOrDefault(ExtraConfigKeys.DisableAutomaticPluginReloading, false))
                Ioc.RegisterMany<Core3AssemblyLoader>(ReuseType.Singleton);
            else
                Ioc.RegisterMany<NetAssemblyLoader>(ReuseType.Singleton);
#else
            Ioc.RegisterMany<NetAssemblyLoader>(ReuseType.Singleton);
#endif
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
        protected virtual void InitializePlugins()
        {
            Ioc.ResolveMany<IPlugin>().ForEach(p => { });
            Ioc.Resolve<ControllerManager>().Initialize();
            Ioc.ResolveMany<IWebsiteStartHandler>().ForEach(h => h.OnWebsiteStart());
        }

        /// <summary>
        /// Start core services<br/>
        /// 开启核心服务<br/>
        /// </summary>
        protected virtual void StartServices()
        {
            var config = Ioc.Resolve<WebsiteConfigManager>().WebsiteConfig;
            if (!config.Extra.GetOrDefault(ExtraConfigKeys.DisableAutomaticPluginReloading, false))
            {
                Ioc.Resolve<PluginReloader>().Start();
            }
            Ioc.Resolve<AutomaticCacheCleaner>().Start();
        }

        /// <summary>
        /// Log emergency error<br/>
        /// 记录紧急错误<br/>
        /// </summary>
        /// <param name="message"></param>
        protected virtual void LogEmergencyError(string message)
        {
            try
            {
                Console.WriteLine(message);
                var rootDirectory = WebsiteRootDirectory;
                if (string.IsNullOrEmpty(rootDirectory))
                {
                    rootDirectory = Path.GetTempPath();
                }
                var logPath = Path.Combine(rootDirectory, "emergencyError.log");
                File.AppendAllText(logPath, message + "\r\n");
            }
            catch
            {
                // ignore any error because it's emergency
            }
        }

        /// <summary>
        /// Intialize main application<br/>
        /// 初始化主应用<br/>
        /// </summary>
        public virtual void Initialize(string websiteRootDirectory)
        {
            if (Interlocked.Exchange(ref _initializeFlag, 1) != 0)
            {
                throw new InvalidOperationException("Don't call Initialize twice");
            }
            try
            {
                WebsiteRootDirectory = websiteRootDirectory;
                InitializeContainer();
                InitializeCoreComponents();
                InitializePlugins();
                StartServices();
                _readyFlag = true;
                Ioc.Resolve<LogManager>().LogInfo("Application Initialized");
            }
            catch (Exception ex)
            {
                var message = $"Initialize application error: {ex.ToDetailedString()}";
                LogEmergencyError(message);
                _initializeException = new InvalidOperationException(message, ex);
                throw _initializeException;
            }
        }

        /// <summary>
        /// Handle http error<br/>
        /// 处理Http错误<br/>
        /// </summary>
        public virtual void OnError(IHttpContext context, Exception ex)
        {
            // Detect nested call
            if (HttpManager.CurrentContextExists)
            {
                throw new InvalidOperationException("Nested call is unsupported");
            }
            // Call error handlers, in reverse register order
            using (HttpManager.OverrideContext(context))
            {
                foreach (var handler in
                    Ioc.ResolveMany<IHttpRequestErrorHandler>().Reverse())
                {
                    handler.OnError(ex);
                }
            }
        }

        /// <summary>
        /// Handle http request<br/>
        /// 处理Http请求<br/>
        /// </summary>
        public virtual void OnRequest(IHttpContext context)
        {
            // Detect whether Initialize is running
            if (!_readyFlag)
            {
                var begin = DateTime.UtcNow;
                while (!_readyFlag)
                {
                    if (_initializeException != null)
                        throw _initializeException;
                    if (DateTime.UtcNow - begin > _waitReadyTimeout)
                        throw new InvalidOperationException(
                            $"Application is not ready (hang at Initialize more than {_waitReadyTimeout.TotalSeconds} seconds)");
                    Thread.Sleep(100);
                }
            }
            // Detect nested call
            if (HttpManager.CurrentContextExists)
            {
                throw new InvalidOperationException("Nested call is unsupported");
            }
            // Call request handlers
            using (HttpManager.OverrideContext(context))
            {
                // Increase requests count
                Interlocked.Increment(ref _inProgressRequests);
                try
                {
                    // Call pre request handlers, in register order
                    foreach (var handler in Ioc.ResolveMany<IHttpRequestPreHandler>())
                    {
                        handler.OnRequest();
                        if (context.Response.IsEnded)
                        {
                            return;
                        }
                    }
                    // Wrap handler action by wrappers
                    var handlerAction = new Action(() =>
                    {
                        // Call request handlers, in reverse register order
                        foreach (var handler in Ioc.ResolveMany<IHttpRequestHandler>().Reverse())
                        {
                            handler.OnRequest();
                            if (context.Response.IsEnded)
                            {
                                return;
                            }
                        }
                        // If request not get handled, throw an 404 exception
                        throw new HttpException(404, "Not Found");
                    });
                    foreach (var wrapper in Ioc.ResolveMany<IHttpRequestHandlerWrapper>())
                    {
                        handlerAction = wrapper.WrapHandlerAction(handlerAction);
                    }
                    handlerAction();
                }
                finally
                {
                    try
                    {
                        // Call post request handlers, in register order
                        foreach (var handler in Ioc.ResolveMany<IHttpRequestPostHandler>())
                        {
                            handler.OnRequest();
                        }
                    }
                    finally
                    {
                        // Decrease requests count
                        Interlocked.Decrement(ref _inProgressRequests);
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
        public virtual IDisposable OverrideIoc()
        {
            var previousOverride = _overrideIoc.Value;
            _overrideIoc.Value = (IContainer)Ioc.Clone();
            return new SimpleDisposable(() =>
            {
                var tmp = _overrideIoc.Value;
                _overrideIoc.Value = previousOverride;
                tmp?.Dispose();
            });
        }

        /// <summary>
        /// Dispose the resources used by the application<br/>
        /// 释放应用使用的资源<br/>
        /// </summary>
        public void Dispose()
        {
            _defaultIoc.Dispose();
            _overrideIoc.Value?.Dispose();
        }

        /// <summary>
        /// Return whether this application is unloadable on this platform<br/>
        /// 返回应用在当前平台上是否支持卸载<br/>
        /// </summary>
        public bool Unloadable
        {
            get
            {
                var pluginManager = Ioc.Resolve<PluginManager>();
                return pluginManager.Unloadable;
            }
        }

        /// <summary>
        /// Unload application, only supported on some platforms<br/>
        /// 卸载应用，仅支持部分平台<br/>
        /// </summary>
        public void Unload()
        {
            Ioc.Resolve<LogManager>().LogInfo("Unload application");
            Ioc.Resolve<PluginManager>().Unload();
            Dispose();
            // clear cache for fast reflection
            ReflectionExtensions.ClearCache();
        }
    }
}

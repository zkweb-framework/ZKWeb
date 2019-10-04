using System;
using ZKWeb.Server;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;

namespace ZKWeb
{
    /// <summary>
    /// Main Application<br/>
    /// 主应用<br/>
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Application Instance<br/>
        /// 应用的实例<br/>
        /// </summary>
        public static IApplication Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                if (_instanceFactory == null)
                {
#pragma warning disable S2372 // Exceptions should not be thrown from property getters
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                    throw new ArgumentNullException("Please set Application.Instance first");
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
#pragma warning restore S2372 // Exceptions should not be thrown from property getters
                }
                lock (_instanceInitializeLock)
                {
                    if (_instance != null)
                    {
                        return _instance;
                    }
                    // intializing process will access Instance property, so assign it first
                    try
                    {
                        _instance = _instanceFactory();
                        _instance.Initialize(_websiteRootDirectory);
                        return _instance;
                    }
                    catch
                    {
                        // unload instance (it must be unloadable), it will up again on next request
                        Unload();
                        throw;
                    }
                }
            }
            set { _instance = value; }
        }
        private static volatile IApplication _instance;
        private static Func<IApplication> _instanceFactory;
        private static object _instanceInitializeLock = new object();
        private static string _websiteRootDirectory;
        /// <summary>
        /// ZKWeb Version String<br/>
        /// ZKWeb的版本字符串<br/>
        /// </summary>
        public static string FullVersion { get { return Instance.FullVersion; } }
        /// <summary>
        /// ZKWeb Version Object<br/>
        /// ZKWeb的版本对象<br/>
        /// </summary>
        public static Version Version { get { return Instance.Version; } }
        /// <summary>
        /// The IoC Container Instance<br/>
        /// IoC容器的实例<br/>
        /// </summary>
        public static IContainer Ioc { get { return Instance.Ioc; } }
        /// <summary>
        /// In progress requests<br/>
        /// 处理中的请求数量<br/>
        /// </summary>
        public static int InProgressRequests { get { return Instance.InProgressRequests; } }

        /// <summary>
        /// Intialize application with DefaultApplication<br/>
        /// 初始化默认应用<br/>
        /// </summary>
        public static void Initialize(string websiteRootDirectory)
        {
            Initialize<DefaultApplication>(websiteRootDirectory);
        }

        /// <summary>
        /// Intialize application with specificed application type<br/>
        /// 初始化指定应用<br/>
        /// </summary>
        public static void Initialize<TApplication>(string websiteRootDirectory)
            where TApplication : IApplication, new()
        {
            Initialize(new TApplication(), websiteRootDirectory);
        }

        /// <summary>
        /// Intialize application with specificed application instance<br/>
        /// 初始化指定应用<br/>
        /// </summary>
        public static void Initialize(IApplication application, string websiteRootDirectory)
        {
            Instance = application;
            Instance.Initialize(websiteRootDirectory);
        }

        /// <summary>
        /// Intialize application with specificed application factory, support automatic reloading<br/>
        /// 初始化指定应用，支持自动重启<br/>
        /// </summary>
        public static void Initialize(Func<IApplication> applicationFactory, string websiteRootDirectory)
        {
            _instanceFactory = applicationFactory;
            _websiteRootDirectory = websiteRootDirectory;
            Instance = applicationFactory();
            Instance.Initialize(websiteRootDirectory);
        }

        /// <summary>
        /// Handle http request<br/>
        /// Method "Response.End" will be called if processing completed without errors
        /// 处理Http请求<br/>
        /// 如果处理成功完成将会调用"Response.End"函数<br/>
        /// </summary>
        public static void OnRequest(IHttpContext context)
        {
            Instance.OnRequest(context);
        }

        /// <summary>
        /// Handle http error<br/>
        /// Method "Response.End" will be called if processing completed without errors<br/>
        /// 处理Http错误<br/>
        /// 如果处理成功完成将会调用"Response.End"函数<br/> 
        /// </summary>
        public static void OnError(IHttpContext context, Exception ex)
        {
            Instance.OnError(context, ex);
        }

        /// <summary>
        /// Override IoC container, only affect the thread calling this method<br/>
        /// Overrided container will inherit original container,<br/>
        /// Alter overrided container will not affect original container.<br/>
        /// 重载当前线程中的IoC容器, 只影响调用此函数的线程<br/>
        /// 重载后的容器会继承原始容器的内容, 并且修改重载后的容器不会影响原始容器<br/>
        /// </summary>
        public static IDisposable OverrideIoc()
        {
            return Instance.OverrideIoc();
        }

        /// <summary>
        /// Return whether this application is unloadable on this platform<br/>
        /// 返回应用在当前平台上是否支持卸载<br/>
        /// </summary>
        public static bool Unloadable { get { return Instance.Unloadable; } }

        /// <summary>
        /// Unload application, only supported on some platforms<br/>
        /// 卸载应用，仅支持部分平台<br/>
        /// </summary>
        public static void Unload()
        {
            var instance = _instance;
            if (instance != null)
                instance.Unload();
            _instance = null;
        }
    }
}

using System;
using System.FastReflection;
using System.Linq;
using System.Reflection;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;
using ZKWebStandard.Ioc.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web
{
    /// <summary>
    /// Controller manager<br/>
    /// 控制器管理器<br/>
    /// </summary>
    /// <seealso cref="IController"/>
    /// <example>
    /// <code language="cs">
    /// var controllerManager = Application.Ioc.Resolve&lt;IControllerManager&gt;();
    /// controllerManager.RegisterAction("example", "GET", () => new PlainResult("abc"));
    /// var action = controllerManager.GetAction("example", "GET");
    /// </code>
    /// </example>
    public class ControllerManager : IHttpRequestHandler
    {
        /// <summary>
        /// The suffix of controller type name<br/>
        /// 控制器类型名称的后缀<br/>
        /// </summary>
        public static readonly string ControllerSuffix = "Controller";
        /// <summary>
        /// The index method name<br/>
        /// 首页函数名称<br/>
        /// </summary>
        public static readonly string IndexMethodName = "Index";
        /// <summary>
        /// Action Collection<br/>
        /// Action函数的集合<br/>
        /// </summary>
        public IActionCollection Actions { get; private set; }
        /// <summary>
        /// Disable case sensitive routing<br/>
        /// 禁止大小写敏感的路由<br/>
        /// </summary>
        public bool DisableCaseSensitiveRouting { get; private set; }

        /// <summary>
        /// Initialize<br/>
        /// 初始化<br/>
        /// </summary>
        public ControllerManager()
        {
            Actions = Application.Ioc.Resolve<IActionCollection>();
            DisableCaseSensitiveRouting = Application.Ioc.Resolve<WebsiteConfigManager>()
                .WebsiteConfig.Extra.GetOrDefault<bool>(ExtraConfigKeys.DisableCaseSensitiveRouting);
        }

        /// <summary>
        /// Handle http request<br/>
        /// Find the action from the request path, if not found then do nothing<br/>
        /// 处理Http请求<br/>
        /// 根据请求路径查找对应的Action函数，如果找不到则不做任何事情<br/>
        /// </summary>
        public virtual void OnRequest()
        {
            var context = HttpManager.CurrentContext;
            var action = GetAction(context.Request.Path, context.Request.Method);
            if (action != null)
            {
                var result = action();
                if (!context.Response.IsEnded)
                {
                    // Write response
                    result.WriteResponse(context.Response);
                }
                if (result is IDisposable disposable)
                {
                    // If result is disposable, dispose it
                    disposable.Dispose();
                }
                if (!context.Response.IsEnded)
                {
                    // End response
                    context.Response.End();
                }
            }
        }

        /// <summary>
        /// Register controller factory data<br/>
        /// 注册控制器工厂函数<br/>
        /// Rules:<br/>
        /// Class without [ActionBase], method with [Action("abc")] => /abc (for backward compatibility)<br/>
        /// Class without [ActionBase], method without [Action] => /$controller/$action<br/>
        /// Class without [ActionBase], method Index without [Action] => /$controller, /$controller/Index<br/>
        /// Class with [ActionBase("abc")], method with [Action("index")] => /abc/index<br/>
        /// Class with [ActionBase("abc")], method without [Action] => /abc/$action<br/>
        /// Class with [ActionBase("abc")], method Index without [Action] => /abc, /abc/Index<br/>
        /// </summary>
        public virtual void RegisterController(ContainerFactoryData factoryData)
        {
            var type = factoryData.ImplementationTypeHint;
            Func<IController> factory = () => (IController)factoryData.GetInstance(Application.Ioc, type);
            // Calculate path base from attribute or controller name - suffix
            var actionBaseAttribute = type.GetCustomAttribute<ActionBaseAttribute>();
            string pathBase;
            if (actionBaseAttribute?.PathBase != null)
            {
                pathBase = actionBaseAttribute.PathBase.TrimEnd('/');
            }
            else
            {
                pathBase = type.Name.EndsWith(ControllerSuffix) ?
                    type.Name.Substring(0, type.Name.Length - ControllerSuffix.Length) :
                    type.Name;
            }
            foreach (var method in type.FastGetMethods(
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                // Ignore special and void methods
                if (method.IsSpecialName || method.ReturnType == typeof(void))
                {
                    continue;
                }
                // Get action attributes
                var actionAttributes = method.GetCustomAttributes<ActionAttribute>();
                if (!actionAttributes.Any())
                {
                    if (method.Name == IndexMethodName)
                    {
                        actionAttributes = new[] { new ActionAttribute(), new ActionAttribute("") };
                    }
                    else
                    {
                        actionAttributes = new[] { new ActionAttribute() };
                    }
                }
                // Build action
                var action = factory.BuildActionDelegate(method);
                // Apply action filters
                var filterAttributes = method.GetCustomAttributes<ActionFilterAttribute>();
                foreach (var filterAttribute in filterAttributes)
                {
                    action = filterAttribute.Filter(action);
                }
                // Register action
                foreach (var attribute in actionAttributes)
                {
                    string path;
                    if (actionBaseAttribute == null)
                    {
                        if (attribute.Path != null)
                        {
                            path = attribute.Path;
                        }
                        else
                        {
                            path = pathBase + "/" + method.Name;
                        }
                    }
                    else
                    {
                        if (attribute.Path != null)
                        {
                            path = pathBase + "/" + attribute.Path.TrimStart('/');
                        }
                        else
                        {
                            path = pathBase + "/" + method.Name;
                        }
                    }
                    var httpMethod = attribute.Method ?? HttpMethods.GET;
                    RegisterAction(path, httpMethod, action, attribute.OverrideExists);
                }
            }
        }

        /// <summary>
        /// Register controller type, reuse type will be Transient<br/>
        /// 注册控制器类型, 重用类型是Transient<br/>
        /// </summary>
        public virtual void RegisterController(Type type)
        {
            var factoryData = new ContainerFactoryData(
                ContainerFactoryBuilder.BuildFactory(type),
                ReuseType.Transient,
                type);
            RegisterController(factoryData);
        }

        /// <summary>
        /// Register controller type, reuse type will be Transient<br/>
        /// 注册控制器类型, 重用类型是Transient<br/>
        /// </summary>
        public virtual void RegisterController<T>()
        {
            RegisterController(typeof(T));
        }

        /// <summary>
        /// Register controller instance, reuse type will be Singleton<br/>
        /// 注册控制器实例, 重用类型是Singleton<br/>
        /// </summary>
        public virtual void RegisterController(IController controller)
        {
            var factoryData = new ContainerFactoryData(
                (c, s) => controller,
                ReuseType.Singleton,
                controller.GetType());
            RegisterController(factoryData);
        }

        /// <summary>
        /// Normalize path<br/>
        /// If path not startswith / then add /<br/>
        /// If path ends / then remove /<br/>
        /// 正规化路径<br/>
        /// 如果路径不以/开始则添加/到开始<br/>
        /// 如果路径以/结尾则删除结尾的/</summary>
        /// <param name="path">Path</param>
        /// <returns></returns>
        public virtual string NormalizePath(string path)
        {
            if (path.Length > 1 && path.EndsWith("/"))
            {
                path = path.TrimEnd('/');
            }
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            if (DisableCaseSensitiveRouting)
            {
                path = path.ToLowerInvariant();
            }
            return path;
        }

        /// <summary>
        /// Register action<br/>
        /// 注册Action函数<br/>
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="method">Method</param>
        /// <param name="action">Action</param>
        public virtual void RegisterAction(string path, string method, Func<IActionResult> action)
        {
            RegisterAction(path, method, action, false);
        }

        /// <summary>
        /// Register action<br/>
        /// 注册Action函数<br/>
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="method">Method</param>
        /// <param name="action">Action</param>
        /// <param name="overrideExists">Allow override exists action</param>
        public virtual void RegisterAction(
            string path, string method, Func<IActionResult> action, bool overrideExists)
        {
            // Apply global registered action filter
            var actionFilters = Application.Ioc.ResolveMany<IActionFilter>();
            foreach (var filter in actionFilters)
            {
                action = filter.Filter(action);
            }
            // Associate path and method with action
            path = NormalizePath(path);
            Actions.Set(path, method, action, overrideExists);
        }

        /// <summary>
        /// Unregister action<br/>
        /// 注销Action函数<br/>
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="method">Method</param>
        /// <returns></returns>
        public virtual bool UnregisterAction(string path, string method)
        {
            path = NormalizePath(path);
            return Actions.Remove(path, method);
        }

        /// <summary>
        /// Get action from path and method<br/>
        /// Return null if not found<br/>
        /// 根据路径和方法获取Action函数<br/>
        /// 找不到则返回null<br/>
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="method">Method</param>
        /// <returns></returns>
        public virtual Func<IActionResult> GetAction(string path, string method)
        {
            path = NormalizePath(path);
            return Actions.Get(path, method);
        }

        /// <summary>
        /// Initialize<br/>
        /// Add controllers registered to IoC container<br/>
        /// Attention: all registered controllers will be created here<br/>
        /// 初始化<br/>
        /// 添加已注册到IoC容器的控制器<br/>
        /// 注意: 所有已注册的控制器都会在这里被创建<br/>
        /// </summary>
        internal protected virtual void Initialize()
        {
            foreach (var factories in Application.Ioc.ResolveFactories(typeof(IController)))
            {
                RegisterController(factories);
            }
        }
    }
}

using System;
using System.FastReflection;
using System.Linq;
using System.Reflection;
using ZKWebStandard.Ioc;
using ZKWebStandard.Ioc.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web {
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
	public class ControllerManager : IHttpRequestHandler {
		/// <summary>
		/// Action Collection<br/>
		/// Action函数的集合<br/>
		/// </summary>
		public IActionCollection Actions { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public ControllerManager() {
			Actions = Application.Ioc.Resolve<IActionCollection>();
		}

		/// <summary>
		/// Handle http request<br/>
		/// Find the action from the request path, if not found then do nothing<br/>
		/// 处理Http请求<br/>
		/// 根据请求路径查找对应的Action函数，如果找不到则不做任何事情<br/>
		/// </summary>
		public virtual void OnRequest() {
			var context = HttpManager.CurrentContext;
			var action = GetAction(context.Request.Path, context.Request.Method);
			if (action != null) {
				var result = action();
				// Write response
				result.WriteResponse(context.Response);
				// If result is disposable, dispose it
				if (result is IDisposable) {
					((IDisposable)result).Dispose();
				}
				// End response
				context.Response.End();
			}
		}

		/// <summary>
		/// Register controller factory data<br/>
		/// 注册控制器工厂函数<br/>
		/// </summary>
		public virtual void RegisterController(ContainerFactoryData factoryData) {
			// Get all public methods with ActionAttribute
			var type = factoryData.ImplementationTypeHint;
			var factory = (Func<IController>)factoryData.GenericFactory;
			foreach (var method in type.FastGetMethods(
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
				// Get action attributes
				var actionAttributes = method.GetCustomAttributes<ActionAttribute>();
				if (!actionAttributes.Any()) {
					continue;
				}
				// Build action
				var action = factory.BuildActionDelegate(method);
				// Apply action filters
				var filterAttributes = method.GetCustomAttributes<ActionFilterAttribute>();
				foreach (var filterAttribute in filterAttributes) {
					action = filterAttribute.Filter(action);
				}
				// Register action
				foreach (var attribute in actionAttributes) {
					RegisterAction(attribute.Path, attribute.Method, action, attribute.OverrideExists);
				}
			}
		}

		/// <summary>
		/// Register controller type, reuse type will be Transient<br/>
		/// 注册控制器类型, 重用类型是Transient<br/>
		/// </summary>
		public virtual void RegisterController(Type type) {
			Application.Ioc.NonGenericBuildAndWrapFactory(
				type, ReuseType.Transient, out var genericFactory, out var objectFactory);
			var factoryData = new ContainerFactoryData(genericFactory, objectFactory, type);
			RegisterController(factoryData);
		}

		/// <summary>
		/// Register controller type, reuse type will be Transient<br/>
		/// 注册控制器类型, 重用类型是Transient<br/>
		/// </summary>
		public virtual void RegisterController<T>() {
			var genericFactory = Application.Ioc.GenericBuildAndWrapFactory<T>(ReuseType.Transient);
			var objectFactory = new Func<object>(() => genericFactory());
			var factoryData = new ContainerFactoryData(genericFactory, objectFactory, typeof(T));
			RegisterController(factoryData);
		}

		/// <summary>
		/// Register controller instance, reuse type will be Singleton<br/>
		/// 注册控制器实例, 重用类型是Singleton<br/>
		/// </summary>
		public virtual void RegisterController(IController controller) {
			Application.Ioc.NonGenericWrapFactory(
				controller.GetType(), () => controller, ReuseType.Transient,
				out var genericFactory, out var objectFactory);
			var factoryData = new ContainerFactoryData(
				genericFactory, objectFactory, controller.GetType());
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
		public virtual string NormalizePath(string path) {
			if (path.Length > 1 && path.EndsWith("/")) {
				path = path.TrimEnd('/');
			}
			if (!path.StartsWith("/")) {
				path = "/" + path;
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
		public virtual void RegisterAction(string path, string method, Func<IActionResult> action) {
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
			string path, string method, Func<IActionResult> action, bool overrideExists) {
			// Apply global registered action filter
			var actionFilters = Application.Ioc.ResolveMany<IActionFilter>();
			foreach (var filter in actionFilters) {
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
		public virtual bool UnregisterAction(string path, string method) {
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
		public virtual Func<IActionResult> GetAction(string path, string method) {
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
		internal protected virtual void Initialize() {
			foreach (var factories in Application.Ioc.ResolveFactories(typeof(IController))) {
				RegisterController(factories);
			}
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using System.Reflection;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web {
	/// <summary>
	/// Controller manager<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="IController"/>
	public class ControllerManager : IHttpRequestHandler {
		/// <summary>
		/// { (Path, Method): Action }
		/// </summary>
		protected IDictionary<Pair<string, string>, Func<IActionResult>> Actions { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		public ControllerManager() {
			Actions = new ConcurrentDictionary<Pair<string, string>, Func<IActionResult>>();
		}

		/// <summary>
		/// Handle http request<br/>
		/// Find the action from the request path, if not found then do nothing<br/>
		/// <br/>
		/// <br/>
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
		/// Register controller type<br/>
		/// <br/>
		/// </summary>
		[Obsolete("Use RegisterController(controller)")]
		public virtual void RegisterController<T>() {
			RegisterController(typeof(T));
		}

		/// <summary>
		/// Register controller type<br/>
		/// <br/>
		/// </summary>
		[Obsolete("Use RegisterController(controller)")]
		public virtual void RegisterController(Type type) {
			RegisterController((IController)Activator.CreateInstance(type));
		}

		/// <summary>
		/// Register controller instance<br/>
		/// Attention: this instance will be used across all requests<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="controller">Controller instance</param>
		public virtual void RegisterController(IController controller) {
			// Get all public methods with ActionAttribute
			var type = controller.GetType();
			foreach (var method in type.FastGetMethods(
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
				// Get action attributes
				var actionAttributes = method.GetCustomAttributes<ActionAttribute>();
				if (!actionAttributes.Any()) {
					continue;
				}
				// Build action
				var action = controller.BuildActionDelegate(method);
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
		/// Normalize path<br/>
		/// If path not startswith / then add /<br/>
		/// If path ends / then remove /<br/>
		/// <br/>
		/// <br/>
		/// </summary>
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
		/// <br/>
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="method">Method</param>
		/// <param name="action">Action</param>
		public virtual void RegisterAction(string path, string method, Func<IActionResult> action) {
			RegisterAction(path, method, action, false);
		}

		/// <summary>
		/// Register action<br/>
		/// <br/>
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="method">Method</param>
		/// <param name="action">Action</param>
		/// <param name="overrideExists">Allow override exist actions</param>
		public virtual void RegisterAction(
			string path, string method, Func<IActionResult> action, bool overrideExists) {
			// Apply global registered action filter
			var actionFilters = Application.Ioc.ResolveMany<IActionFilter>();
			foreach (var filter in actionFilters) {
				action = filter.Filter(action);
			}
			// Associate path and method with action
			path = NormalizePath(path);
			var key = Pair.Create(path, method);
			if (!overrideExists && Actions.ContainsKey(key)) {
				throw new ArgumentException($"action for {path} already registered, try option `overrideExists`");
			}
			Actions[key] = action;
		}

		/// <summary>
		/// Unregister action<br/>
		/// <br/>
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="method">Method</param>
		/// <returns></returns>
		public virtual bool UnregisterAction(string path, string method) {
			path = NormalizePath(path);
			var key = Pair.Create(path, method);
			return Actions.Remove(key);
		}

		/// <summary>
		/// Get action from path and method<br/>
		/// Return null if not found<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="method">Method</param>
		/// <returns></returns>
		public virtual Func<IActionResult> GetAction(string path, string method) {
			path = NormalizePath(path);
			var key = Pair.Create(path, method);
			return Actions.GetOrDefault(key);
		}

		/// <summary>
		/// Initialize<br/>
		/// Add registered controllers<br/>
		/// Attention: all controllers will be created here<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		internal static void Initialize() {
			var controllerManager = Application.Ioc.Resolve<ControllerManager>();
			foreach (var controller in Application.Ioc.ResolveMany<IController>()) {
				controllerManager.RegisterController(controller);
			}
		}
	}
}

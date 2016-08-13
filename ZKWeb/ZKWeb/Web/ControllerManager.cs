using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Web;

namespace ZKWeb.Web {
	/// <summary>
	/// Controller manager
	/// </summary>
	public class ControllerManager : IHttpRequestHandler {
		/// <summary>
		/// { (Path, Method): Action }
		/// </summary>
		protected IDictionary<Pair<string, string>, Func<IActionResult>> Actions { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public ControllerManager() {
			Actions = new ConcurrentDictionary<Pair<string, string>, Func<IActionResult>>();
		}

		/// <summary>
		/// Handle http request
		/// Find the action from the request path, if not found then do nothing
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
		/// Register controller
		/// </summary>
		/// <typeparam name="T">Controller type</typeparam>
		public virtual void RegisterController<T>() {
			RegisterController(typeof(T));
		}

		/// <summary>
		/// Register controller
		/// </summary>
		/// <param name="type">Controller type</param>
		public virtual void RegisterController(Type type) {
			// Get all public methods with ActionAttribute
			foreach (var method in type.FastGetMethods(
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
				var attributes = method.GetCustomAttributes<ActionAttribute>();
				if (attributes.Any()) {
					// Compile it to delegate for performance
					var makeInstance = method.IsStatic ? null :
						Expression.New(type.GetTypeInfo().GetConstructors()[0]);
					Func<IActionResult> action;
					var actionResultType = typeof(IActionResult).GetTypeInfo();
					if (actionResultType.IsAssignableFrom(method.ReturnType)) {
						action = Expression.Lambda<Func<IActionResult>>(
							Expression.Call(makeInstance, method)).Compile();
					} else {
						var plainResultType = typeof(PlainResult).GetTypeInfo();
						action = Expression.Lambda<Func<IActionResult>>(
							Expression.New(plainResultType.GetConstructors()[0],
							Expression.Call(makeInstance, method))).Compile();
					}
					// Register action
					foreach (var attribute in attributes) {
						RegisterAction(attribute.Path, attribute.Method, action, attribute.OverrideExists);
					}
				}
			}
		}

		/// <summary>
		/// Normalize path
		/// If path not startswith / then add /
		/// If path ends / then remove /
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
		/// Register action
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="method">Method</param>
		/// <param name="action">Action</param>
		public virtual void RegisterAction(string path, string method, Func<IActionResult> action) {
			RegisterAction(path, method, action, false);
		}

		/// <summary>
		/// Register action
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="method">Method</param>
		/// <param name="action">Action</param>
		/// <param name="overrideExists">Allow override exist actions</param>
		public virtual void RegisterAction(
			string path, string method, Func<IActionResult> action, bool overrideExists) {
			path = NormalizePath(path);
			var key = Pair.Create(path, method);
			if (!overrideExists && Actions.ContainsKey(key)) {
				throw new ArgumentException($"action for {path} already registered, try option `overrideExists`");
			}
			Actions[key] = action;
		}

		/// <summary>
		/// Unregister action
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
		/// Get action from path and method
		/// Return null if not found
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
		/// Initialize
		/// Add registered controllers
		/// </summary>
		internal static void Initialize() {
			var controllerManager = Application.Ioc.Resolve<ControllerManager>();
			foreach (var controller in Application.Ioc.ResolveMany<IController>()) {
				controllerManager.RegisterController(controller.GetType());
			}
		}
	}
}

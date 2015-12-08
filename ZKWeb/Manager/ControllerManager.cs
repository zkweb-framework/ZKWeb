using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Model.ActionResults;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Manager {
	/// <summary>
	/// 控制器管理器
	/// </summary>
	public class ControllerManager : IApplicationRequestHandler {
		/// <summary>
		/// { (路径, 类型): 处理函数, ... }
		/// </summary>
		private Dictionary<Tuple<string, string>, Func<IActionResult>>
			Actions { get; set; } =
			new Dictionary<Tuple<string, string>, Func<IActionResult>>();

		/// <summary>
		/// 处理http请求
		/// 查找路径对应的处理函数，存在时使用该函数否则跳过处理
		/// </summary>
		public void OnRequest() {
			var context = HttpContext.Current;
			var action = Actions.GetOrDefault(
				Tuple.Create(context.Request.Path, context.Request.HttpMethod));
			if (action != null) {
				action().WriteResponse(context.Response);
				context.Response.End();
			}
		}

		/// <summary>
		/// 注册控制器类型
		/// </summary>
		/// <typeparam name="T">控制器类型</typeparam>
		public void RegisterController<T>() {
			RegisterController(typeof(T));
		}

		/// <summary>
		/// 注册控制器类型
		/// </summary>
		/// <param name="type">控制器类型</param>
		public void RegisterController(Type type) {
			// 枚举所有带ActionAttribute的属性的公开函数
			foreach (var method in type.GetMethods(
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
				var attributes = method.GetCustomAttributes<ActionAttribute>();
				if (attributes.Any()) {
					// 预先编译好函数，提高实际调用时的性能
					var makeInstance = method.IsStatic ? null :
						Expression.New(type.GetConstructors()[0]);
					Func<IActionResult> function;
					if (typeof(IActionResult).IsAssignableFrom(method.ReturnType)) {
						function = Expression.Lambda<Func<IActionResult>>(
							Expression.Call(makeInstance, method)).Compile();
					} else {
						function = Expression.Lambda<Func<IActionResult>>(
							Expression.New(typeof(PlainResult).GetConstructors()[0],
							Expression.Call(makeInstance, method))).Compile();
					}
					// 添加函数
					foreach (var attribute in attributes) {
						var key = Tuple.Create(attribute.Path, attribute.Method);
						Actions[key] = function;
					}
				}
			}
		}
	}

	/// <summary>
	/// 标记函数可以处理指定路径的http请求
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ActionAttribute : Attribute {
		/// <summary>
		/// 路径
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// 请求类型
		/// </summary>
		public string Method { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">路径，前面没有/时会自动添加</param>
		/// <param name="method">请求类型，默认是GET</param>
		public ActionAttribute(string path, string method = ActionMethods.GET) {
			Path = path.StartsWith("/") ? path : ("/" + path);
			Method = method;
		}
	}

	/// <summary>
	/// 常用的http的请求类型定义
	/// </summary>
	public static class ActionMethods {
		/// <summary>
		/// GET
		/// </summary>
		public const string GET = "GET";
		/// <summary>
		/// POST
		/// </summary>
		public const string POST = "POST";
		/// <summary>
		/// PUT
		/// </summary>
		public const string PUT = "PUT";
		/// <summary>
		/// DELETE
		/// </summary>
		public const string DELETE = "DELETE";
		/// <summary>
		/// PATCH
		/// </summary>
		public const string PATCH = "PATCH";
	}
}

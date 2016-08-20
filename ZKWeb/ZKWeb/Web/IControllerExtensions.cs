using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web {
	/// <summary>
	/// IController extension methods
	/// </summary>
	public static class IControllerExtensions {
		/// <summary>
		/// Method use to get request parameter
		/// </summary>
		/// <typeparam name="T">Parameter type</typeparam>
		/// <param name="name">Parameter name</param>
		/// <returns></returns>
		private static T GetRequestParameter<T>(string name) {
			return HttpManager.CurrentContext.Request.Get<T>(name);
		}

		/// <summary>
		/// Method information of GetRequestParameter
		/// </summary>
		private static MethodInfo GetRequestParameterMethod =>
			typeof(IControllerExtensions).GetMethod("GetRequestParameter",
				BindingFlags.NonPublic | BindingFlags.Static);

		/// <summary>
		/// Build action delegate from method information
		/// Result type handling
		/// - If method returns IActionResult, then use it
		/// - If method returns string, then wrap result with PlainResult
		/// - Otherwise wrap result with JsonResult
		/// Parameters handling
		/// - Get parameter by it's name from http request
		/// - There no null check about parameters
		/// </summary>
		/// <param name="controller">Controller instance</param>
		/// <param name="method">Method information</param>
		/// <returns></returns>
		public static Func<IActionResult> BuildActionDelegate(
			this IController controller, MethodInfo method) {
			var parameters = method.GetParameters();
			var instanceExpr = method.IsStatic ? null : Expression.Constant(controller);
			var parametersExpr = new List<Expression>();
			foreach (var parameter in parameters) {
				// Get parameters from request by it's name
				parametersExpr.Add(Expression.Call(null,
					GetRequestParameterMethod.MakeGenericMethod(parameter.ParameterType),
					Expression.Constant(parameter.Name)));
			}
			// Determines if we need wrap the result
			var actionResultType = typeof(IActionResult).GetTypeInfo();
			if (actionResultType.IsAssignableFrom(method.ReturnType)) {
				return Expression.Lambda<Func<IActionResult>>(
					Expression.Call(instanceExpr, method, parametersExpr)).Compile();
			} else if (method.ReturnType == typeof(string)) {
				var plainResultType = typeof(PlainResult).GetTypeInfo();
				return Expression.Lambda<Func<IActionResult>>(
					Expression.New(plainResultType.GetConstructors()[0],
						Expression.Call(instanceExpr, method, parametersExpr))).Compile();
			} else {
				var jsonResultType = typeof(JsonResult).GetTypeInfo();
				return Expression.Lambda<Func<IActionResult>>(
					Expression.New(jsonResultType.GetConstructors()[0],
						Expression.Call(instanceExpr, method, parametersExpr),
						Expression.Constant(Formatting.None))).Compile();
			}
		}
	}
}

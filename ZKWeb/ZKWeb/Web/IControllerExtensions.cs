using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Web {
	/// <summary>
	/// IController extension methods
	/// </summary>
	public static class IControllerExtensions {
		/// <summary>
		/// Get action parameter
		/// </summary>
		/// <typeparam name="T">Parameter type</typeparam>
		/// <param name="name">Parameter name</param>
		/// <param name="method">Method information</param>
		/// <param name="parameterInfo">Parameter information</param>
		/// <returns></returns>
		internal static T GetActionParameter<T>(
			string name, MethodInfo method, ParameterInfo parameterInfo) {
			var provider = Application.Ioc.Resolve<IActionParameterProvider>();
			return provider.GetParameter<T>(name, method, parameterInfo);
		}

		/// <summary>
		/// Method information of GetActionParameter
		/// </summary>
		private static MethodInfo GetActionParameterMethod =>
			typeof(IControllerExtensions).GetMethod(nameof(GetActionParameter),
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
					GetActionParameterMethod.MakeGenericMethod(parameter.ParameterType),
					Expression.Constant(parameter.Name),
					Expression.Constant(method),
					Expression.Constant(parameter)));
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

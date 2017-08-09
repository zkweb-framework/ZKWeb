using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Web {
	/// <summary>
	/// IController extension methods<br/>
	/// 控制器的扩展函数<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public static class IControllerExtensions {
		/// <summary>
		/// Get action parameter<br/>
		/// 获取Action参数<br/>
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
		/// Method information of GetActionParameter<br/>
		/// GetActionParameter的MethodInfo对象<br/>
		/// </summary>
		private readonly static MethodInfo GetActionParameterMethod = typeof(IControllerExtensions)
			.GetMethod(nameof(GetActionParameter), BindingFlags.NonPublic | BindingFlags.Static);

		/// <summary>
		/// Build action delegate from method information<br/>
		/// Result type handling<br/>
		/// - If method returns IActionResult, then use it<br/>
		/// - If method returns string, then wrap result with PlainResult<br/>
		/// - Otherwise wrap result with JsonResult<br/>
		/// Parameters handling<br/>
		/// - Get parameter by it's name from http request<br/>
		/// - There no null check about parameters<br/>
		/// 根据Action函数的信息构建Action委托<br/>
		/// 结果类型的处理<br/>
		/// - 如果函数返回IActionResult, 则使用返回的实例<br/>
		/// - 如果函数返回string, 则包装结果到PlainResult<br/>
		/// - 其他情况则包装结果到JsonResult<br/>
		/// 参数的处理<br/>
		/// - 根据参数的名称从http请求中获取参数值<br/>
		/// - 不会检查参数是否为null<br/>
		/// </summary>
		/// <param name="controllerFactory">Controller factory</param>
		/// <param name="method">Method information</param>
		/// <returns></returns>
		public static Func<IActionResult> BuildActionDelegate(
			this Func<IController> controllerFactory, MethodInfo method) {
			var parameters = method.GetParameters();
			var instanceExpr = method.IsStatic ? null :
				Expression.Convert(
					Expression.Invoke(Expression.Constant(controllerFactory)),
					method.DeclaringType);
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
			var actionResultType = typeof(IActionResult);
			if (actionResultType.IsAssignableFrom(method.ReturnType)) {
				return Expression.Lambda<Func<IActionResult>>(
					Expression.Call(instanceExpr, method, parametersExpr)).Compile();
			} else if (method.ReturnType == typeof(string)) {
				var plainResultType = typeof(PlainResult);
				return Expression.Lambda<Func<IActionResult>>(
					Expression.New(plainResultType.GetConstructors()[0],
						Expression.Call(instanceExpr, method, parametersExpr))).Compile();
			} else {
				var jsonResultType = typeof(JsonResult);
				return Expression.Lambda<Func<IActionResult>>(
					Expression.New(jsonResultType.GetConstructors()[0],
						Expression.Convert(
							Expression.Call(instanceExpr, method, parametersExpr),
							typeof(object)),
						Expression.Constant(Formatting.None))).Compile();
			}
		}

		/// <summary>
		/// Build action delegate from method information<br/>
		/// 根据Action函数的信息构建Action委托<br/>
		/// </summary>
		/// <param name="controller">Controller instance</param>
		/// <param name="method">Method information</param>
		/// <returns></returns>
		/// <seealso cref="BuildActionDelegate(Func{IController}, MethodInfo)"/>
		public static Func<IActionResult> BuildActionDelegate(
			this IController controller, MethodInfo method) {
			var controllerFactory = new Func<IController>(() => controller);
			return controllerFactory.BuildActionDelegate(method);
		}
	}
}

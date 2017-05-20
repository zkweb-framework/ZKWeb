using System;
using System.Reflection;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionParameterProviders {
	/// <summary>
	/// Default action parameter provider<br/>
	/// 默认的Action参数提供器<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public class DefaultActionParameterProvider : IActionParameterProvider {
		/// <summary>
		/// Get parameter from http request<br/>
		/// 从Http请求获取参数<br/>
		/// </summary>
		public T GetParameter<T>(string name, MethodInfo method, ParameterInfo parameterInfo) {
			// Get parameter from form or query
			var request = HttpManager.CurrentContext.Request;
			var result = request.Get<T>(name);
			if (result != null) {
				return result;
			}
			// Get parameter from all form or query values if type isn't basic type
			var typeInfo = typeof(T).GetTypeInfo();
			if (!typeInfo.IsValueType && typeof(T) != typeof(string) &&
				!(typeInfo.IsGenericType &&
				typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))) {
				return request.GetAllAs<T>();
			}
			// Return default value
			return default(T);
		}
	}
}

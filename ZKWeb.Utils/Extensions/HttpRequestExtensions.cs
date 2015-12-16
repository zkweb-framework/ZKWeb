using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// Http请求类的扩展函数
	/// </summary>
	public static class HttpRequestExtensions {
		/// <summary>
		/// 判断http请求是否由ajax发起
		/// </summary>
		/// <param name="request">http请求</param>
		/// <returns></returns>
		public static bool IsAjaxRequest(this HttpRequest request) {
			if (request == null) {
				return false;
			} else if (request["X-Requested-With"] == "XMLHttpRequest") {
				return true;
			} else if (request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest") {
				return true;
			}
			return false;
		}

		/// <summary>
		/// 获取http请求中的指定参数值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request">http请求</param>
		/// <param name="key">参数名称</param>
		/// <param name="defaultValue">获取失败时的默认值</param>
		/// <returns></returns>
		public static T GetParam<T>(this HttpRequest request, string key, T defaultValue) {
			var value = request.Params[key];
			if (string.IsNullOrEmpty(value)) {
				return defaultValue;
			}
			return value.ConvertOrDefault<T>(defaultValue);
		}

		/// <summary>
		/// 获取http请求中的所有参数
		/// </summary>
		/// <param name="request">http请求</param>
		/// <returns></returns>
		public static Dictionary<string, string> GetParams(this HttpRequest request) {
			var result = new Dictionary<string, string>();
			foreach (string key in request.Params.Keys) {
				result[key] = request.Params[key];
			}
			return result;
		}
	}
}

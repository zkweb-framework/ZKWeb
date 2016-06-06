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
	public static class HttpRequestBaseExtensions {
		/// <summary>
		/// 判断http请求是否由ajax发起
		/// </summary>
		/// <param name="request">http请求</param>
		/// <returns></returns>
		public static bool IsAjaxRequest(this HttpRequestBase request) {
			if (request == null || request.Headers == null) {
				return false;
			}
			return request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		/// <summary>
		/// 获取http请求中的指定参数值
		/// 优先级: 请求字符串 > 表单内容 > Cookies > 服务器变量
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request">http请求</param>
		/// <param name="key">参数名称</param>
		/// <param name="defaultValue">获取失败时的默认值</param>
		/// <returns></returns>
		public static T Get<T>(this HttpRequestBase request, string key, T defaultValue = default(T)) {
			var value = request[key];
			if (string.IsNullOrEmpty(value)) {
				return defaultValue;
			}
			return value.ConvertOrDefault<T>(defaultValue);
		}

		/// <summary>
		/// 获取http请求中的所有参数
		/// 优先级: 请求字符串 > 表单内容 > Cookies > 服务器变量
		/// </summary>
		/// <param name="request">http请求</param>
		/// <returns></returns>
		public static Dictionary<string, string> GetAll(this HttpRequestBase request) {
			var result = new Dictionary<string, string>();
			request.ServerVariables.AllKeys.ForEach(key => result[key] = request.ServerVariables[key]);
			request.Cookies.OfType<HttpCookie>().ForEach(c => result[c.Name] = c.Value);
			request.Form.AllKeys.ForEach(key => result[key] = request.Form[key]);
			request.QueryString.AllKeys.ForEach(key => result[key] = request.QueryString[key]);
			return result;
		}
	}
}

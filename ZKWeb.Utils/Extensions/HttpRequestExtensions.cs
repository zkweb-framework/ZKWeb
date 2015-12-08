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
	}
}

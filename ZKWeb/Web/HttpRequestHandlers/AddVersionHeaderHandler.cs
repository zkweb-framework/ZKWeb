using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Functions;
using ZKWeb.Web.Interfaces;

namespace ZKWeb.Web.HttpRequestHandlers {
	/// <summary>
	/// 添加版本头信息到回应
	/// </summary>
	public class AddVersionHeaderHandler : IHttpRequestPreHandler {
		/// <summary>
		/// 处理请求
		/// </summary>
		public void OnRequest() {
			var response = HttpContextUtils.CurrentContext.Response;
			response.AddHeader("X-ZKWeb-Version", Application.FullVersion);
		}
	}
}

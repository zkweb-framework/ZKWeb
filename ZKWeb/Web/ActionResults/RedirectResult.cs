using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Web.Interfaces;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// 重定向结果
	/// </summary>
	public class RedirectResult : IActionResult {
		/// <summary>
		/// 重定向到的url
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// 是否永久重定向
		/// </summary>
		public bool Permanent { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="url">重定向到的url</param>
		/// <param name="permanent">是否永久重定向</param>
		public RedirectResult(string url, bool permanent = false) {
			Url = url;
			Permanent = permanent;
		}

		/// <summary>
		/// 写入到http回应
		/// </summary>
		/// <param name="response"></param>
		public void WriteResponse(HttpResponse response) {
			if (Permanent) {
				response.RedirectPermanent(Url);
			} else {
				response.Redirect(Url);
			}
		}
	}
}

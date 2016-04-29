using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// Http回应类的扩展函数
	/// </summary>
	public static class HttpResponseBaseExtensions {
		/// <summary>
		/// 通过脚本跳转到指定url
		/// 用这个代替301跳转可以保留referer
		/// </summary>
		/// <param name="response">Http回应</param>
		/// <param name="url">跳转到的url地址</param>
		public static void RedirectByScript(this HttpResponseBase response, string url) {
			var urlJson = JsonConvert.SerializeObject(url);
			response.Clear();
			response.ContentType = "text/html";
			response.Write($@"<script type='text/javascript'>location.href = {urlJson};</script>");
			response.End();
		}
	}
}

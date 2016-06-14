using ZKWeb.Web.Abstractions;
using ZKWebStandard.Web;

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
		/// 跳转到指定地址
		/// </summary>
		/// <param name="response">Http回应</param>
		public void WriteResponse(IHttpResponse response) {
			response.Redirect(Url, Permanent);
		}
	}
}

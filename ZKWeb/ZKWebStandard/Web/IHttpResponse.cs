using System.IO;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http response<br/>
	/// Http回应的接口<br/>
	/// </summary>
	public interface IHttpResponse {
		/// <summary>
		/// Reponse body<br/>
		/// 回应内容<br/>
		/// </summary>
		Stream Body { get; }
		/// <summary>
		/// Content type<br/>
		/// 内容类型<br/>
		/// </summary>
		string ContentType { get; set; }
		/// <summary>
		/// Parent http context<br/>
		/// 所属的Http上下文<br/>
		/// </summary>
		IHttpContext HttpContext { get; }
		/// <summary>
		/// Status code<br/>
		/// 状态代码<br/>
		/// </summary>
		int StatusCode { get; set; }

		/// <summary>
		/// Set cookie value<br/>
		/// 设置Cookie值<br/>
		/// </summary>
		/// <param name="key">Cookie key</param>
		/// <param name="value">Cookie value</param>
		/// <param name="options">Options for setting cookie</param>
		void SetCookie(string key, string value, HttpCookieOptions options);
		/// <summary>
		/// Add http header<br/>
		/// 添加Http头<br/>
		/// </summary>
		/// <param name="key">Header key</param>
		/// <param name="value">Header value</param>
		void AddHeader(string key, string value);
		/// <summary>
		/// Redirect to url<br/>
		/// 重定向到Url<br/>
		/// </summary>
		/// <param name="url">Url address</param>
		/// <param name="permanent">Is permanent redirect</param>
		void Redirect(string url, bool permanent);
		/// <summary>
		/// End response<br/>
		/// It should throw an exception to stop processing<br/>
		/// 结束回应<br/>
		/// 它会抛出一个例外用于停止继续处理<br/>
		/// </summary>
		void End();
	}
}

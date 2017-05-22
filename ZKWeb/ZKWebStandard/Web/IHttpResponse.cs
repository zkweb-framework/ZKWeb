using System.IO;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http response<br/>
	/// <br/>
	/// </summary>
	public interface IHttpResponse {
		/// <summary>
		/// Reponse body<br/>
		/// <br/>
		/// </summary>
		Stream Body { get; }
		/// <summary>
		/// Content type<br/>
		/// <br/>
		/// </summary>
		string ContentType { get; set; }
		/// <summary>
		/// Parent http context<br/>
		/// <br/>
		/// </summary>
		IHttpContext HttpContext { get; }
		/// <summary>
		/// Status code<br/>
		/// <br/>
		/// </summary>
		int StatusCode { get; set; }

		/// <summary>
		/// Set cookie value<br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Cookie key</param>
		/// <param name="value">Cookie value</param>
		/// <param name="options">Options for setting cookie</param>
		void SetCookie(string key, string value, HttpCookieOptions options);
		/// <summary>
		/// Add http header<br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Header key</param>
		/// <param name="value">Header value</param>
		void AddHeader(string key, string value);
		/// <summary>
		/// Redirect to url<br/>
		/// <br/>
		/// </summary>
		/// <param name="url">Url address</param>
		/// <param name="permanent">Is permanent redirect</param>
		void Redirect(string url, bool permanent);
		/// <summary>
		/// End response<br/>
		/// It should throw an exception to stop processing<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		void End();
	}
}

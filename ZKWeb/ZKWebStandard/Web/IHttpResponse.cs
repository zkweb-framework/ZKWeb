using System.IO;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http response
	/// </summary>
	public interface IHttpResponse {
		/// <summary>
		/// Reponse body
		/// </summary>
		Stream Body { get; }
		/// <summary>
		/// Content type
		/// </summary>
		string ContentType { get; set; }
		/// <summary>
		/// Parent http context
		/// </summary>
		IHttpContext HttpContext { get; }
		/// <summary>
		/// Status code
		/// </summary>
		int StatusCode { get; set; }

		/// <summary>
		/// Set cookie value
		/// </summary>
		/// <param name="key">Cookie key</param>
		/// <param name="value">Cookie value</param>
		/// <param name="options">Options for setting cookie</param>
		void SetCookie(string key, string value, HttpCookieOptions options);
		/// <summary>
		/// Add http header
		/// </summary>
		/// <param name="key">Header key</param>
		/// <param name="value">Header value</param>
		void AddHeader(string key, string value);
		/// <summary>
		/// Redirect to url
		/// </summary>
		/// <param name="url">Url address</param>
		/// <param name="permanent">Is permanent redirect</param>
		void Redirect(string url, bool permanent);
		/// <summary>
		/// End response
		/// It should throw an exception to stop processing
		/// </summary>
		void End();
	}
}

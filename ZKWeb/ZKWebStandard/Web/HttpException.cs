using System;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http exception<br/>
	/// Please use this type instead of Asp.Net and Asp.Net Core's<br/>
	/// Http例外<br/>
	/// 请使用这个类型代替Asp.Net和Asp.Net Core中的类型<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// throw new HttpException(404, "Not Found");
	/// </code>
	/// </example>
	public class HttpException : Exception {
		/// <summary>
		/// Status code<br/>
		/// 状态码<br/>
		/// </summary>
		public int StatusCode { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="statusCode">Status code</param>
		/// <param name="message">Exception message</param>
		public HttpException(int statusCode, string message = null) : base(message) {
			StatusCode = statusCode;
		}
	}
}

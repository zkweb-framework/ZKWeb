using System;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http exception<br/>
	/// Please use this type instead of Asp.Net and Asp.Net Core's<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	public class HttpException : Exception {
		/// <summary>
		/// Status code<br/>
		/// <br/>
		/// </summary>
		public int StatusCode { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="statusCode">Status code</param>
		/// <param name="message">Exception message</param>
		public HttpException(int statusCode, string message = null) : base(message) {
			StatusCode = statusCode;
		}
	}
}

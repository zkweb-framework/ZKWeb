using System;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http exception
	/// Please use this type instead of Asp.Net and Asp.Net Core's
	/// </summary>
	public class HttpException : Exception {
		/// <summary>
		/// Status code
		/// </summary>
		public int StatusCode { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="statusCode">Status code</param>
		/// <param name="message">Exception message</param>
		public HttpException(int statusCode, string message = null) : base(message) {
			StatusCode = statusCode;
		}
	}
}

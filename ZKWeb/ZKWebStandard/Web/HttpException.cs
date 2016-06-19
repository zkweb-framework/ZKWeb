using System;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http错误
	/// </summary>
	public class HttpException : Exception {
		/// <summary>
		/// 状态代码
		/// </summary>
		public int StatusCode { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="statusCode">状态代码</param>
		/// <param name="message">例外消息</param>
		public HttpException(int statusCode, string message = null) : base(message) {
			StatusCode = statusCode;
		}
	}
}

using System;

namespace ZKWeb.Web.Abstractions {
	/// <summary>
	/// 请求错误的处理器的接口
	/// </summary>
	public interface IHttpRequestErrorHandler {
		/// <summary>
		/// 处理请求错误
		/// </summary>
		/// <param name="ex">例外对象</param>
		/// <returns></returns>
		void OnError(Exception ex);
	}
}

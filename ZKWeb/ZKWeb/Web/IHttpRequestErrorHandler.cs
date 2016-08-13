using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface for http request error handler
	/// </summary>
	public interface IHttpRequestErrorHandler {
		/// <summary>
		/// Handle request error
		/// </summary>
		/// <param name="ex">Exception object</param>
		/// <returns></returns>
		void OnError(Exception ex);
	}
}

using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface for http request error handler<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="IHttpRequestHandler"/>
	/// <seealso cref="IHttpRequestPreHandler"/>
	/// <seealso cref="IHttpRequestPostHandler"/>
	/// <seealso cref="IHttpRequestHandlerWrapper"/>
	public interface IHttpRequestErrorHandler {
		/// <summary>
		/// Handle request error<br/>
		/// <br/>
		/// </summary>
		/// <param name="ex">Exception object</param>
		/// <returns></returns>
		void OnError(Exception ex);
	}
}

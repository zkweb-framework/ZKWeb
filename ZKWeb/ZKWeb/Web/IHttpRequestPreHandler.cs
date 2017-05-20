namespace ZKWeb.Web {
	/// <summary>
	/// Interface for http request handler<br/>
	/// Call in register order, before calling IHttpRequestHandler<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	/// <seealso cref="IHttpRequestPostHandler"/>
	/// <seealso cref="IHttpRequestHandlerWrapper"/>
	/// <seealso cref="IHttpRequestHandler"/>
	public interface IHttpRequestPreHandler {
		/// <summary>
		/// Handle http reqeust<br/>
		/// It's not recommend to call response.End here<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		void OnRequest();
	}
}

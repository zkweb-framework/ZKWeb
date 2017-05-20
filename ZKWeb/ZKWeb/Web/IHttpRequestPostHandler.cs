namespace ZKWeb.Web {
	/// <summary>
	/// Interface for http request handler<br/>
	/// Call in register order, after calling IHttpRequestHandler<br/>
	/// Usually used to cleanup things done in IHttpResquestPreHandler<br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	/// <seealso cref="IHttpRequestPreHandler"/>
	/// <seealso cref="IHttpRequestHandlerWrapper"/>
	/// <seealso cref="IHttpRequestHandler"/>
	public interface IHttpRequestPostHandler {
		/// <summary>
		/// Handle http reqeust<br/>
		/// It's not recommend to call response.End here<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		void OnRequest();
	}
}

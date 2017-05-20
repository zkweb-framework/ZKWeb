namespace ZKWeb.Web {
	/// <summary>
	/// Interface for http request handler<br/>
	/// Call in reversed register order, after called IHttpRequestPreHandler<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	/// <seealso cref="IHttpRequestPreHandler"/>
	/// <seealso cref="IHttpRequestPostHandler"/>
	/// <seealso cref="IHttpRequestHandlerWrapper"/>
	public interface IHttpRequestHandler {
		/// <summary>
		/// Handle http reqeust<br/>
		/// Can call response.End to end response immediately<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		void OnRequest();
	}
}

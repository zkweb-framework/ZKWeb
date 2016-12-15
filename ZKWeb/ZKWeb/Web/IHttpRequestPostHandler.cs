namespace ZKWeb.Web {
	/// <summary>
	/// Interface for http request handler
	/// Call in register order, after calling IHttpRequestHandler
	/// Usually used to cleanup things done in IHttpResquestPreHandler
	/// </summary>
	public interface IHttpRequestPostHandler {
		/// <summary>
		/// Handle http reqeust
		/// It's not recommend to call response.End here
		/// </summary>
		void OnRequest();
	}
}

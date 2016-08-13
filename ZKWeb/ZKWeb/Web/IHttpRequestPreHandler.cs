namespace ZKWeb.Web {
	/// <summary>
	/// Interface for http request handler
	/// Call in register order, before calling IHttpRequestHandler
	/// </summary>
	public interface IHttpRequestPreHandler {
		/// <summary>
		/// Handle http reqeust
		/// It's not recommend to call response.End here
		/// </summary>
		void OnRequest();
	}
}

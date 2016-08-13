namespace ZKWeb.Web {
	/// <summary>
	/// Interface for http request handler
	/// Call in reversed register order, after called IHttpRequestPreHandler
	/// </summary>
	public interface IHttpRequestHandler {
		/// <summary>
		/// Handle http reqeust
		/// Can call response.End to end response immediately
		/// </summary>
		void OnRequest();
	}
}

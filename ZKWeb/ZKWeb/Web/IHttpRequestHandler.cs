namespace ZKWeb.Web {
	/// <summary>
	/// Interface of http request handler<br/>
	/// Call in reversed register order, after called IHttpRequestPreHandler<br/>
	/// Http请求处理器的接口<br/>
	/// 按注册顺序的相反顺序调用, 在调用完IHttpRequestPreHandler以后调用<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	/// <seealso cref="IHttpRequestPreHandler"/>
	/// <seealso cref="IHttpRequestPostHandler"/>
	/// <seealso cref="IHttpRequestHandlerWrapper"/>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public class ExampleRequestHandler : IHttpRequestHandler {
	///		public void OnRequest() {
	///			var request = HttpManager.CurrentContext.Request;
	///			var response = HttpManager.CurrentContext.Response;
	///			if (request.Path == "/example") {
	///				response.Write("example result");
	///				response.End();
	///			}
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IHttpRequestHandler {
		/// <summary>
		/// Handle http reqeust<br/>
		/// Can call method response.End to end response immediately<br/>
		/// 处理Http请求<br/>
		/// 可以调用response.End函数来立刻终止回应<br/>
		/// </summary>
		void OnRequest();
	}
}

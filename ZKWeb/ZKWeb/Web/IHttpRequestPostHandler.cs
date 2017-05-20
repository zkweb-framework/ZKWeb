namespace ZKWeb.Web {
	/// <summary>
	/// Interface of http request post handler<br/>
	/// Call in register order, after calling IHttpRequestHandler<br/>
	/// Http请求的后处理器的接口<br/>
	/// 按注册顺序调用, 在调用完IHttpRequestHandler以后调用<br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	/// <seealso cref="IHttpRequestPreHandler"/>
	/// <seealso cref="IHttpRequestHandlerWrapper"/>
	/// <seealso cref="IHttpRequestHandler"/>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public class ExampleRequestPostHandler : IHttpRequestPostHandler {
	///		public void OnRequest() {
	///			var logManager = Application.Ioc.Resolve&lt;LogManager&gt;();
	///			logManager.LogDebug("request finished");
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IHttpRequestPostHandler {
		/// <summary>
		/// Handle http reqeust<br/>
		/// It's not recommend to call method response.End here<br/>
		/// 处理http请求<br/>
		/// 不推荐在这里调用response.End函数<br/>
		/// </summary>
		void OnRequest();
	}
}

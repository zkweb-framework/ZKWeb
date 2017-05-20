namespace ZKWeb.Web {
	/// <summary>
	/// Interface of http request pre handler<br/>
	/// Call in register order, before calling IHttpRequestHandler<br/>
	/// Http请求的预处理器的接口<br/>
	/// 按注册顺序调用, 在调用IHttpRequestHanler前调用<br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	/// <seealso cref="IHttpRequestPostHandler"/>
	/// <seealso cref="IHttpRequestHandlerWrapper"/>
	/// <seealso cref="IHttpRequestHandler"/>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public class ExampleRequestPreHandler : IHttpRequestPreHandler {
	///		public void OnRequest() {
	///			var logManager = Application.Ioc.Resolve&lt;LogManager&gt;();
	///			logManager.LogDebug("request started");
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IHttpRequestPreHandler {
		/// <summary>
		/// Handle http reqeust<br/>
		/// It's not recommend to call response.End here<br/>
		/// 处理http请求<br/>
		/// 不推荐在这里调用response.End函数<br/>
		/// </summary>
		void OnRequest();
	}
}

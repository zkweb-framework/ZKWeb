using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface of http request error handler<br/>
	/// Call in reversed register order<br/>
	/// Http请求错误处理器的接口<br/>
	/// 按注册顺序的相反顺序调用<br/>
	/// </summary>
	/// <seealso cref="IHttpRequestHandler"/>
	/// <seealso cref="IHttpRequestPreHandler"/>
	/// <seealso cref="IHttpRequestPostHandler"/>
	/// <seealso cref="IHttpRequestHandlerWrapper"/>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public ExampleErrorHandler : IHttpRequestErrorHandler {
	///		public void OnError(Exception ex) {
	///			if (ex is HttpException &amp;&amp; ((HttpException)ex).StatusCode == 404) {
	///				var response = HttpManager.CurrentContext.Response;
	///				response.Write("custom 404 message");
	///				response.End();
	///			}
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IHttpRequestErrorHandler {
		/// <summary>
		/// Handle request error<br/>
		/// 处理请求错误<br/>
		/// </summary>
		/// <param name="ex">Exception object</param>
		/// <returns></returns>
		void OnError(Exception ex);
	}
}

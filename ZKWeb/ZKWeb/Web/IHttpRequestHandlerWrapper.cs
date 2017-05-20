using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface used to wrap http request handler<br/>
	/// Call in register order, usually used to override http context<br/>
	/// 用于包装Http请求处理器的接口<br/>
	/// 按注册顺序调用, 通常用于重载Http请求<br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	/// <seealso cref="IHttpRequestPreHandler"/>
	/// <seealso cref="IHttpRequestPostHandler"/>
	/// <seealso cref="IHttpRequestHandler"/>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public class ExampleRequestHandlerWrapper : IHttpRequestHandlerWrapper {
	///		public Action WrapHandlerAction(Action action) {
	///			return () => {
	///				using (HttpManager.OverrideContext(new MyHttpContext())) {
	///					action();
	///				}
	///			};
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IHttpRequestHandlerWrapper {
		/// <summary>
		/// Wrap http request handler<br/>
		/// 包装Http请求处理器<br/>
		/// </summary>
		/// <param name="action">Handler Action</param>
		/// <returns></returns>
		Action WrapHandlerAction(Action action);
	}
}

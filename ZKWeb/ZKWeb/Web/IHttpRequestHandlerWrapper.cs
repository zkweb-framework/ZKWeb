using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface for wrap http request handler action<br/>
	/// Call in register order, usually used to override http context<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="IHttpRequestErrorHandler"/>
	/// <seealso cref="IHttpRequestPreHandler"/>
	/// <seealso cref="IHttpRequestPostHandler"/>
	/// <seealso cref="IHttpRequestHandler"/>
	public interface IHttpRequestHandlerWrapper {
		/// <summary>
		/// Wrap http request handler action<br/>
		/// <br/>
		/// </summary>
		/// <param name="action">Handler Action</param>
		/// <returns></returns>
		Action WrapHandlerAction(Action action);
	}
}

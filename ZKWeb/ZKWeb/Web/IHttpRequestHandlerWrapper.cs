using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface for wrap http request handler action
	/// Call in register order, usually used to override http context
	/// </summary>
	public interface IHttpRequestHandlerWrapper {
		/// <summary>
		/// Wrap http request handler action
		/// </summary>
		/// <param name="action">Handler Action</param>
		/// <returns></returns>
		Action WrapHandlerAction(Action action);
	}
}

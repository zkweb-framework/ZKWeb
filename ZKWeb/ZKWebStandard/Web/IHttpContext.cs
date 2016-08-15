using System.Collections.Generic;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http context
	/// </summary>
	public interface IHttpContext {
		/// <summary>
		/// Http request
		/// </summary>
		IHttpRequest Request { get; }
		/// <summary>
		/// Http response
		/// </summary>
		IHttpResponse Response { get; }
		/// <summary>
		/// Http context bound items
		/// </summary>
		IDictionary<object, object> Items { get; }
	}
}

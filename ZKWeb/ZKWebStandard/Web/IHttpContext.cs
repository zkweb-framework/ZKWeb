using System.Collections.Generic;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http context<br/>
	/// <br/>
	/// </summary>
	public interface IHttpContext {
		/// <summary>
		/// Http request<br/>
		/// <br/>
		/// </summary>
		IHttpRequest Request { get; }
		/// <summary>
		/// Http response<br/>
		/// <br/>
		/// </summary>
		IHttpResponse Response { get; }
		/// <summary>
		/// Http context bound items<br/>
		/// <br/>
		/// </summary>
		IDictionary<object, object> Items { get; }
	}
}

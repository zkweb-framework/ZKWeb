using System.Collections.Generic;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for http context<br/>
	/// Http上下文的接口<br/>
	/// </summary>
	public interface IHttpContext {
		/// <summary>
		/// Http request<br/>
		/// Http请求<br/>
		/// </summary>
		IHttpRequest Request { get; }
		/// <summary>
		/// Http response<br/>
		/// Http回应<br/>
		/// </summary>
		IHttpResponse Response { get; }
		/// <summary>
		/// Http context bound items<br/>
		/// 绑定到上下文的对象集合<br/>
		/// </summary>
		IDictionary<object, object> Items { get; }
	}
}

using System.Collections.Generic;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http上下文的接口
	/// </summary>
	public interface IHttpContext {
		/// <summary>
		/// 对应的请求
		/// </summary>
		IHttpRequest Request { get; }
		/// <summary>
		/// 对应的回应
		/// </summary>
		IHttpResponse Response { get; }
		/// <summary>
		/// 同一上下文中共享的对象
		/// </summary>
		IDictionary<object, object> Items { get; }
	}
}

using System.Collections.Generic;
using ZKWebStandard.Web;
using System.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Http context wrapper for Asp.Net<br/>
	/// Asp.Net Http上下文的包装类<br/>
	/// </summary>
	internal class AspNetHttpContextWrapper : IHttpContext {
		/// <summary>
		/// Original http context<br/>
		/// 原始的Http上下文<br/>
		/// </summary>
		protected HttpContext OriginalContext { get; set; }
		/// <summary>
		/// Wrapped http request<br/>
		/// 包装后的Http请求<br/>
		/// </summary>
		protected AspNetHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// Wrapped http response<br/>
		/// 包装后的Http回应<br/>
		/// </summary>
		protected AspNetHttpResponseWrapper ChildResponse { get; set; }
		/// <summary>
		/// Http context bound items<br/>
		/// Items from original context is not using<br/>
		/// 与Http上下文绑定的数据<br/>
		/// 原始上下文中的Items不会使用<br/>
		/// </summary>
		public Dictionary<object, object> ChildItems { get; set; }

		public IHttpRequest Request {
			get { return ChildRequest; }
		}
		public IHttpResponse Response {
			get { return ChildResponse; }
		}
		public IDictionary<object, object> Items {
			get { return ChildItems; }
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="originalContext">Orignal http context</param>
		public AspNetHttpContextWrapper(HttpContext originalContext) {
			OriginalContext = originalContext;
			ChildRequest = new AspNetHttpRequestWrapper(this, originalContext.Request);
			ChildResponse = new AspNetHttpResponseWrapper(this, originalContext.Response);
			ChildItems = new Dictionary<object, object>();
		}
	}
}

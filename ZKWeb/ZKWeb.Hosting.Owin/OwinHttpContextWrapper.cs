using Microsoft.Owin;
using System.Collections.Generic;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Http context wrapper for Owin<br/>
	/// Owin的Http上下文的包装类<br/>
	/// </summary>
	internal class OwinHttpContextWrapper : IHttpContext {
		/// <summary>
		/// Original Http context<br/>
		/// 原始的Http上下文<br/>
		/// </summary>
		protected IOwinContext OwinContext { get; set; }
		/// <summary>
		/// Wrapped http request<br/>
		/// 包装后的Http请求<br/>
		/// </summary>
		protected OwinHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// Wrapped http response<br/>
		/// 包装后的Http回应<br/>
		/// </summary>
		protected OwinHttpResponseWrapper ChildResponse { get; set; }
		/// <summary>
		/// Http context bound items<br/>
		/// Http上下文绑定的数据<br/>
		/// </summary>
		protected Dictionary<object, object> ChildItems { get; set; }

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
		/// <param name="owinContext">Original http context</param>
		public OwinHttpContextWrapper(IOwinContext owinContext) {
			OwinContext = owinContext;
			ChildRequest = new OwinHttpRequestWrapper(this, owinContext.Request);
			ChildResponse = new OwinHttpResponseWrapper(this, owinContext.Response);
			ChildItems = new Dictionary<object, object>();
		}
	}
}

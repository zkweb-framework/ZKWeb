using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Http context wrapper for Asp.Net Core<br/>
	/// Asp.Net Core的Http上下文包装类<br/>
	/// </summary>
	internal class CoreHttpContextWrapper : IHttpContext {
		/// <summary>
		/// Original http context<br/>
		/// 原始的Http上下文<br/>
		/// </summary>
		protected HttpContext CoreContext { get; set; }
		/// <summary>
		/// Wrapped http request<br/>
		/// 包装后的Http请求<br/>
		/// </summary>
		protected CoreHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// Wrapped http response<br/>
		/// 包装后的Http回应<br/>
		/// </summary>
		protected CoreHttpResponseWrapper ChildResponse { get; set; }

		public IHttpRequest Request {
			get { return ChildRequest; }
		}
		public IHttpResponse Response {
			get { return ChildResponse; }
		}
		public IDictionary<object, object> Items {
			get { return CoreContext.Items; }
		}
		public ConnectionInfo Connection {
			get { return CoreContext.Connection; }
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="coreContext">Asp.Net Core http context</param>
		public CoreHttpContextWrapper(HttpContext coreContext) {
			CoreContext = coreContext;
			ChildRequest = new CoreHttpRequestWrapper(this, coreContext.Request);
			ChildResponse = new CoreHttpResponseWrapper(this, coreContext.Response);
		}
	}
}

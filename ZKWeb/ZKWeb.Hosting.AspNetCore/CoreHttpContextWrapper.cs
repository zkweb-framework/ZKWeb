using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Http context wrapper for Asp.Net Core<br/>
	/// <br/>
	/// </summary>
	internal class CoreHttpContextWrapper : IHttpContext {
		/// <summary>
		/// Original http context<br/>
		/// <br/>
		/// </summary>
		protected HttpContext CoreContext { get; set; }
		/// <summary>
		/// Wrapped http request<br/>
		/// <br/>
		/// </summary>
		protected CoreHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// Wrapped http response<br/>
		/// <br/>
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
		/// <br/>
		/// </summary>
		/// <param name="coreContext">Asp.Net Core http context</param>
		public CoreHttpContextWrapper(HttpContext coreContext) {
			CoreContext = coreContext;
			ChildRequest = new CoreHttpRequestWrapper(this, coreContext.Request);
			ChildResponse = new CoreHttpResponseWrapper(this, coreContext.Response);
		}
	}
}

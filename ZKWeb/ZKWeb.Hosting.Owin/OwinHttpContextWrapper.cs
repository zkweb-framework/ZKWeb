using Microsoft.Owin;
using System.Collections.Generic;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Http context wrapper for Owin
	/// </summary>
	internal class OwinHttpContextWrapper : IHttpContext {
		/// <summary>
		/// Original Http context
		/// </summary>
		protected IOwinContext OwinContext { get; set; }
		/// <summary>
		/// Wrapped http request
		/// </summary>
		protected OwinHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// Wrapped http response
		/// </summary>
		protected OwinHttpResponseWrapper ChildResponse { get; set; }
		/// <summary>
		/// Http context bound items
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
		/// Initialize
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

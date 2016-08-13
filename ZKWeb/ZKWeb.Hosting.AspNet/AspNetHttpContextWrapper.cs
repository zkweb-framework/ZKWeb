using System.Collections.Generic;
using ZKWebStandard.Web;
using System.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Http context wrapper for Asp.Net
	/// </summary>
	internal class AspNetHttpContextWrapper : IHttpContext {
		/// <summary>
		/// Original http context
		/// </summary>
		protected HttpContext OriginalContext { get; set; }
		/// <summary>
		/// Wrapped http request
		/// </summary>
		protected AspNetHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// Wrapped http response
		/// </summary>
		protected AspNetHttpResponseWrapper ChildResponse { get; set; }
		/// <summary>
		/// Http context bound items
		/// Items from original context is not using
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
		/// Initialize
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

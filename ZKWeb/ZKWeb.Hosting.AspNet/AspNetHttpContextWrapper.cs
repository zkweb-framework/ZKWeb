using System.Collections.Generic;
using ZKWebStandard.Web;
using System.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Http context wrapper for Asp.Net<br/>
	/// <br/>
	/// </summary>
	internal class AspNetHttpContextWrapper : IHttpContext {
		/// <summary>
		/// Original http context<br/>
		/// <br/>
		/// </summary>
		protected HttpContext OriginalContext { get; set; }
		/// <summary>
		/// Wrapped http request<br/>
		/// <br/>
		/// </summary>
		protected AspNetHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// Wrapped http response<br/>
		/// <br/>
		/// </summary>
		protected AspNetHttpResponseWrapper ChildResponse { get; set; }
		/// <summary>
		/// Http context bound items<br/>
		/// Items from original context is not using<br/>
		/// <br/>
		/// <br/>
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
		/// <br/>
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

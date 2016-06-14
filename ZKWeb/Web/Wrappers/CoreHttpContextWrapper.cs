using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ZKWebStandard.Web;

namespace ZKWeb.Web.Wrappers {
	/// <summary>
	/// 包装AspNetCore的Http上下文
	/// </summary>
	internal class CoreHttpContextWrapper : IHttpContext {
		/// <summary>
		/// AspNetCore的Http上下文
		/// </summary>
		protected HttpContext CoreContext { get; set; }
		/// <summary>
		/// 包装好的Http请求
		/// </summary>
		protected CoreHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// 包装好的Http回应
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
		/// 初始化
		/// </summary>
		/// <param name="coreContext">AspNetCore的Http上下文</param>
		public CoreHttpContextWrapper(HttpContext coreContext) {
			CoreContext = coreContext;
			ChildRequest = new CoreHttpRequestWrapper(this, coreContext.Request);
			ChildResponse = new CoreHttpResponseWrapper(this, coreContext.Response);
		}
	}
}

using Microsoft.Owin;
using System.Collections.Generic;
using ZKWebStandard.Web;

namespace ZKWeb.Owin.Hosting {
	/// <summary>
	/// 包装Owin的Http上下文
	/// </summary>
	internal class OwinHttpContextWrapper : IHttpContext {
		/// <summary>
		/// Owin的Http上下文
		/// </summary>
		protected IOwinContext OwinContext { get; set; }
		/// <summary>
		/// 包装好的Http请求
		/// </summary>
		protected OwinHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// 包装好的Http回应
		/// </summary>
		protected OwinHttpResponseWrapper ChildResponse { get; set; }
		/// <summary>
		/// Http上下文中共享的数据
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
		/// 初始化
		/// </summary>
		/// <param name="owinContext">Owin的Http上下文</param>
		public OwinHttpContextWrapper(IOwinContext owinContext) {
			OwinContext = owinContext;
			ChildRequest = new OwinHttpRequestWrapper(this, owinContext.Request);
			ChildResponse = new OwinHttpResponseWrapper(this, owinContext.Response);
			ChildItems = new Dictionary<object, object>();
		}
	}
}

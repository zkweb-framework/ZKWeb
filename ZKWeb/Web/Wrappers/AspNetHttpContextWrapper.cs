using System.Collections.Generic;
using ZKWebStandard.Web;
using System.Web;

namespace ZKWeb.Web.Wrappers {
	/// <summary>
	/// 包装原始的Http上下文
	/// </summary>
	internal class AspNetHttpContextWrapper : IHttpContext {
		/// <summary>
		/// 原始的Http上下文
		/// </summary>
		protected HttpContext OriginalContext { get; set; }
		/// <summary>
		/// 包装好的Http请求
		/// </summary>
		protected AspNetHttpRequestWrapper ChildRequest { get; set; }
		/// <summary>
		/// 包装好的Http回应
		/// </summary>
		protected AspNetHttpResponseWrapper ChildResponse { get; set; }
		/// <summary>
		/// Http上下文中共享的数据
		/// 不使用原始的上下文的Items
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
		/// 初始化
		/// </summary>
		/// <param name="originalContext">原始的Http上下文</param>
		public AspNetHttpContextWrapper(HttpContext originalContext) {
			OriginalContext = originalContext;
			ChildRequest = new AspNetHttpRequestWrapper(this, originalContext.Request);
			ChildResponse = new AspNetHttpResponseWrapper(this, originalContext.Response);
			ChildItems = new Dictionary<object, object>();
		}
	}
}

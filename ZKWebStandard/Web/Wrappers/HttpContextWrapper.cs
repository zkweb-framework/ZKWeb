using System.Collections.Generic;

namespace ZKWebStandard.Web.Wrappers {
	/// <summary>
	/// Http上下文的包装类
	/// </summary>
	public abstract class HttpContextWrapper : IHttpContext {
#pragma warning disable CS1591
		protected virtual IHttpContext OriginalContext { get; set; }

		public virtual IHttpRequest Request {
			get { return OriginalContext.Request; }
		}
		public virtual IHttpResponse Response {
			get { return OriginalContext.Response; }
		}
		public virtual IDictionary<object, object> Items {
			get { return OriginalContext.Items; }
		}
#pragma warning restore CS1591

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="originalContext">原始的Http上下文</param>
		public HttpContextWrapper(IHttpContext originalContext) {
			OriginalContext = originalContext;
		}
	}
}

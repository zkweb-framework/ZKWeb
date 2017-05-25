using System.Collections.Generic;

namespace ZKWebStandard.Web.Wrappers {
	/// <summary>
	/// Http context wrapper base class<br/>
	/// Http上下文包装类的基类<br/>
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

		public HttpContextWrapper(IHttpContext originalContext) {
			OriginalContext = originalContext;
		}
#pragma warning restore CS1591
	}
}

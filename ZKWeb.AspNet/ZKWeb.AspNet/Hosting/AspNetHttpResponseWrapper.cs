using System.IO;
using ZKWebStandard.Web;
using System.Web;

namespace ZKWeb.AspNet.Hosting {
	/// <summary>
	/// 包装原始的Http回应
	/// </summary>
	internal class AspNetHttpResponseWrapper : IHttpResponse {
		/// <summary>
		/// 所属的Http上下文
		/// </summary>
		protected AspNetHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// 原始的Http回应
		/// </summary>
		protected HttpResponse OriginalResponse { get; set; }

		public Stream Body {
			get { return OriginalResponse.OutputStream; }
		}
		public string ContentType {
			get { return OriginalResponse.ContentType; }
			set { OriginalResponse.ContentType = value; }
		}
		public IHttpContext HttpContext {
			get { return ParentContext; }
		}
		public int StatusCode {
			get { return OriginalResponse.StatusCode; }
			set { OriginalResponse.StatusCode = value; }
		}

		public void SetCookie(string key, string value, HttpCookieOptions options) {
			var cookie = new HttpCookie(key, value);
			if (options.Domain != null) {
				cookie.Domain = options.Domain;
			}
			if (options.Expires != null) {
				cookie.Expires = options.Expires.Value;
			}
			if (options.HttpOnly != null) {
				cookie.HttpOnly = options.HttpOnly.Value;
			}
			if (options.Path != null) {
				cookie.Path = options.Path;
			}
			if (options.Secure != null) {
				cookie.Secure = options.Secure.Value;
			}
			OriginalResponse.Cookies.Add(cookie);
		}
		public void AddHeader(string key, string value) {
			OriginalResponse.Headers.Add(key, value);
		}
		public void Redirect(string url, bool permanent) {
			OriginalResponse.Redirect(url, permanent);
			End();
		}
		public void End() {
			Body.Flush();
			OriginalResponse.End();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="parentContext">所属的Http上下文</param>
		/// <param name="coreResponse">AspNetCore的Http回应</param>
		public AspNetHttpResponseWrapper(
			AspNetHttpContextWrapper parentContext, HttpResponse coreResponse) {
			ParentContext = parentContext;
			OriginalResponse = coreResponse;
		}
	}
}

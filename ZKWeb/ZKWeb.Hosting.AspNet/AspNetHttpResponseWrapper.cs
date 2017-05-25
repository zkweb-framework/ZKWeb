using System.IO;
using ZKWebStandard.Web;
using System.Web;
using System.Diagnostics;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Http response wrapper for Asp.Net<br/>
	/// <br/>
	/// </summary>
	internal class AspNetHttpResponseWrapper : IHttpResponse {
		/// <summary>
		/// Parent http context<br/>
		/// <br/>
		/// </summary>
		protected AspNetHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// Original http response<br/>
		/// <br/>
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
		[DebuggerNonUserCode]
		public void End() {
			Body.Flush();
			OriginalResponse.End();
		}

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="parentContext">Parent http context</param>
		/// <param name="originalResponse">Original http response</param>
		public AspNetHttpResponseWrapper(
			AspNetHttpContextWrapper parentContext, HttpResponse originalResponse) {
			ParentContext = parentContext;
			OriginalResponse = originalResponse;
		}
	}
}

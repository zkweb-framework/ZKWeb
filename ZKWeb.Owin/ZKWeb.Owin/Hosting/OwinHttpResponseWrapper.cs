using Microsoft.Owin;
using System.IO;
using ZKWebStandard.Web;

namespace ZKWeb.Owin.Hosting {
	/// <summary>
	/// 包装Owin的Http回应
	/// </summary>
	internal class OwinHttpResponseWrapper : IHttpResponse {
		/// <summary>
		/// 所属的Http上下文
		/// </summary>
		protected OwinHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// Owin的Http回应
		/// </summary>
		protected IOwinResponse OwinResponse { get; set; }

		public Stream Body {
			get { return OwinResponse.Body; }
		}
		public string ContentType {
			get { return OwinResponse.ContentType; }
			set { OwinResponse.ContentType = value; }
		}
		public IHttpContext HttpContext {
			get { return ParentContext; }
		}
		public int StatusCode {
			get { return OwinResponse.StatusCode; }
			set { OwinResponse.StatusCode = value; }
		}

		public void SetCookie(string key, string value, HttpCookieOptions options) {
			options = options ?? new HttpCookieOptions();
			var coreOptions = new CookieOptions();
			if (options.Domain != null) {
				coreOptions.Domain = options.Domain;
			}
			if (options.Expires != null) {
				coreOptions.Expires = options.Expires;
			}
			if (options.HttpOnly != null) {
				coreOptions.HttpOnly = options.HttpOnly.Value;
			}
			if (options.Path != null) {
				coreOptions.Path = options.Path;
			}
			if (options.Secure != null) {
				coreOptions.Secure = options.Secure.Value;
			}
			OwinResponse.Cookies.Append(key, value, coreOptions);
		}
		public void AddHeader(string key, string value) {
			OwinResponse.Headers.Add(key, new[] { value });
		}
		public void Redirect(string url, bool permanent) {
			// Owin不支持永久跳转，这里自己写Http头
			OwinResponse.Headers.Add("Location", new[] { url });
			OwinResponse.StatusCode = permanent ? 301 : 302;
			End();
		}
		public void End() {
			Body.Flush();
			throw new OwinHttpResponseEndException();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="parentContext">所属的Http上下文</param>
		/// <param name="owinResponse">Owin的Http回应</param>
		public OwinHttpResponseWrapper(
			OwinHttpContextWrapper parentContext, IOwinResponse owinResponse) {
			ParentContext = parentContext;
			OwinResponse = owinResponse;
		}
	}
}

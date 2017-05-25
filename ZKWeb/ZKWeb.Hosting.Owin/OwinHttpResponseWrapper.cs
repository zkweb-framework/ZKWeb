using Microsoft.Owin;
using System.Diagnostics;
using System.IO;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Http response wrapper for Owin<br/>
	/// Owin的Http回应的包装类<br/>
	/// </summary>
	internal class OwinHttpResponseWrapper : IHttpResponse {
		/// <summary>
		/// Parent http context<br/>
		/// 所属的Http上下文<br/>
		/// </summary>
		protected OwinHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// Original http response<br/>
		/// 原始的Http回应<br/>
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
			// Owin not support permanent direct
			// Just write the header manually
			OwinResponse.Headers.Add("Location", new[] { url });
			OwinResponse.StatusCode = permanent ? 301 : 302;
			End();
		}
		[DebuggerNonUserCode]
		public void End() {
			Body.Flush();
			throw new OwinHttpResponseEndException();
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="parentContext">Parent http context</param>
		/// <param name="owinResponse">Original http response</param>
		public OwinHttpResponseWrapper(
			OwinHttpContextWrapper parentContext, IOwinResponse owinResponse) {
			ParentContext = parentContext;
			OwinResponse = owinResponse;
		}
	}
}

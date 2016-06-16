using System.IO;
using Microsoft.AspNetCore.Http;
using ZKWebStandard.Web;
using System.Threading;

namespace ZKWeb.Web.Wrappers {
	internal class CoreHttpResponseWrapper : IHttpResponse {
		/// <summary>
		/// 所属的Http上下文
		/// </summary>
		protected CoreHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// AspNetCore的Http回应
		/// </summary>
		protected HttpResponse CoreResponse { get; set; }

		public Stream Body {
			get { return CoreResponse.Body; }
		}
		public long? ContentLength {
			get { return CoreResponse.ContentLength; }
			set { CoreResponse.ContentLength = value; }
		}
		public string ContentType {
			get { return CoreResponse.ContentType; }
			set { CoreResponse.ContentType = value; }
		}
		public bool HasStarted {
			get { return CoreResponse.HasStarted; }
		}
		public IHttpContext HttpContext {
			get { return ParentContext; }
		}
		public int StatusCode {
			get { return CoreResponse.StatusCode; }
			set { CoreResponse.StatusCode = value; }
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
			CoreResponse.Cookies.Append(key, value, coreOptions);
		}
		public void AddHeader(string key, string value) {
			CoreResponse.Headers.Add(key, value);
		}
		public void Redirect(string url, bool permanent) {
			CoreResponse.Redirect(url, permanent);
			End();
		}
		public void End() {
			Body.Flush();
			Thread.CurrentThread.Abort();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="parentContext">所属的Http上下文</param>
		/// <param name="coreResponse">AspNetCore的Http回应</param>
		public CoreHttpResponseWrapper(
			CoreHttpContextWrapper parentContext, HttpResponse coreResponse) {
			ParentContext = parentContext;
			CoreResponse = coreResponse;
		}
	}
}

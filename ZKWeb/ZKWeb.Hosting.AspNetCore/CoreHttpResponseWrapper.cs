using System.IO;
using Microsoft.AspNetCore.Http;
using ZKWebStandard.Web;
using System;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Http response wrapper for Asp.Net Core
	/// </summary>
	internal class CoreHttpResponseWrapper : IHttpResponse {
		/// <summary>
		/// Parent http context
		/// </summary>
		protected CoreHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// Original http response
		/// </summary>
		protected HttpResponse CoreResponse { get; set; }

		public Stream Body {
			get { return CoreResponse.Body; }
		}
		public string ContentType {
			get { return CoreResponse.ContentType; }
			set { CoreResponse.ContentType = value; }
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
			// Fix kesterl 1.0.0 304 => 502 error
			// See https://github.com/aspnet/KestrelHttpServer/issues/952
			try {
				if (Body.Position > 0) {
					Body.Flush();
				} else {
					try { CoreResponse.ContentLength = 0; } catch (InvalidOperationException) { }
				}
			} catch (NotSupportedException) {
				// This exception will throw when access Position property if no contents writed before.
				try { CoreResponse.ContentLength = 0; } catch (InvalidOperationException) { }
			}
			throw new CoreHttpResponseEndException();
		}

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="parentContext">Parent http context</param>
		/// <param name="coreResponse">Original http response</param>
		public CoreHttpResponseWrapper(
			CoreHttpContextWrapper parentContext, HttpResponse coreResponse) {
			ParentContext = parentContext;
			CoreResponse = coreResponse;
		}
	}
}
